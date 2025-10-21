using System;
using System.Collections.Generic;
using RsLib;
using RsLib.Adapter;
using RsLib.Builder;
using UnityEngine;
using UnityEngine.UI;

namespace RsTransferPort
{
    public class PortChannelDiagram : MonoBehaviour
    {
        public static void InitPrefab() {
            RsResources.AddLoadPrefabTask("ui/port_overlay_diagram", (parent) =>
            {
                GameObject root = RsUIBuilder.UIGameObject(nameof(PortChannelDiagram), parent, false);
                root.name = nameof(PortChannelDiagram);
                VerticalLayoutGroup layoutGroup = root.AddComponent<VerticalLayoutGroup>();

                layoutGroup.childControlHeight = true;
                layoutGroup.childControlWidth = true;
                
                RectTransform rectTransform = root.rectTransform();
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.offsetMax = new Vector2();
                rectTransform.offsetMin = new Vector2();

                PortChannelDiagram diagram = root.AddComponent<PortChannelDiagram>();

                RsMultiToggleGroupCom lineToggleGroup = root.AddComponent<RsMultiToggleGroupCom>();
                lineToggleGroup.toggles = new[]
                {
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_Hide_LINE, 0, root),
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_CENTER_LINE, 0, root),
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_NEAR_LINE, 0, root),
                };

                RsUIBuilder.BlockLine(root, 16);
                
                //禁止连线动画
                MultiToggle disableLineAnim =
                    RsUIBuilder.ToggleEntry(STRINGS.UI.TOOLS.FILTERLAYERS.RS_DISABLE_LINE_ANIM,
                            ToolParameterMenu.ToggleState.Off, root)
                        .GetComponentInChildren<MultiToggle>();
                
                MultiToggle showPriorityInfo =
                    RsUIBuilder.ToggleEntry(STRINGS.UI.TOOLS.FILTERLAYERS.RS_SHOW_PRIORITY,
                            ToolParameterMenu.ToggleState.Off, root)
                        .GetComponentInChildren<MultiToggle>();

                GameObject moreDetail = RsUIBuilder.UIGameObject("MoreDetail", root);
                VerticalLayoutGroup moreDetailLayoutGroup =
                    moreDetail.AddComponent<VerticalLayoutGroup>();

                moreDetailLayoutGroup.childControlHeight = true;
                moreDetailLayoutGroup.childControlWidth = true;
                moreDetailLayoutGroup.padding = new RectOffset(0,0,16,0);
                
             
                MultiToggle showOnlyNameToggle =
                    RsUIBuilder.ToggleEntry(STRINGS.UI.TOOLS.FILTERLAYERS.RS_ONLY_NULL_CHANNEL,
                            ToolParameterMenu.ToggleState.Off, moreDetail)
                        .GetComponentInChildren<MultiToggle>();
                
                // RsUIBuilder.BlockLine(moreDetail, 16);

                if (DlcManager.IsExpansion1Active())
                {
                    MultiToggle showOnlyGlobalToggle =
                        RsUIBuilder.ToggleEntry(STRINGS.UI.TOOLS.FILTERLAYERS.RS_ONLY_GLOBAL_CHANNEL,
                                ToolParameterMenu.ToggleState.Off, moreDetail)
                            .GetComponentInChildren<MultiToggle>();
                    diagram.showOnlyGlobalToggle = showOnlyGlobalToggle;

                }
                
                RsUIBuilder.BlockLine(moreDetail, 16);

                RsMultiToggleGroupCom typeToggleGroup = root.AddComponent<RsMultiToggleGroupCom>();
                List<MultiToggle> multiToggles = new List<MultiToggle>();
                multiToggles.AddRange(new MultiToggle[]
                {
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_ALL_PORT, 0, moreDetail),
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_GAS_PORT, 0, moreDetail),
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_LIQUID_PORT, 0, moreDetail),
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_SOLID_PORT, 0, moreDetail),
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_POWER_PORT, 0, moreDetail),
                    RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_LOGIC_PORT, 0, moreDetail),
                });
                if (DlcManager.IsExpansion1Active())
                {
                    multiToggles.Add(RsUIBuilder.ToggleEntryToMultiToggle(STRINGS.UI.TOOLS.FILTERLAYERS.RS_HEP_PORT, 0, moreDetail));
                }
                typeToggleGroup.toggles = multiToggles.ToArray();

                diagram.lineToggleGroup = lineToggleGroup;
                diagram.showOnlyNameToggle = showOnlyNameToggle;
                diagram.showPriorityInfoToggle = showPriorityInfo;
                diagram.typeToggleGroup = typeToggleGroup;
                diagram.moreDetailParent = moreDetail;
                diagram.disableLineAnimToggle = disableLineAnim;
                root.SetActive(true);
                
                return root;
            });
        }
        public static GameObject Prefab => RsResources.Load<GameObject>("ui/port_overlay_diagram");

        
        // [SerializeField] private MultiToggle centralConnectionToggle;
        // [SerializeField] private MultiToggle nearbyConnectionToggle;
        // [SerializeField] private MultiToggle noneConnectionToggle;

        [SerializeField] private RsMultiToggleGroupCom lineToggleGroup;
        
        [SerializeField] private RsMultiToggleGroupCom typeToggleGroup;

        [SerializeField] private MultiToggle showOnlyNameToggle;
        
        [SerializeField] private MultiToggle showOnlyGlobalToggle;
        
        [SerializeField] private MultiToggle showPriorityInfoToggle;
        [SerializeField] private MultiToggle disableLineAnimToggle;

        [SerializeField] private GameObject moreDetailParent;
        
        public void Start()
        {
            lineToggleGroup.onSelected += ToggleWiredPreviewMode;
            typeToggleGroup.onSelected += ToggleBuildingType;

            if (showOnlyNameToggle != null)
            {
                showOnlyNameToggle.onClick += ToggleShowOnlyName;
            }

            if (showOnlyGlobalToggle != null)
            {
                showOnlyGlobalToggle.onClick += ToggleGlobalChannel;
            }

            showPriorityInfoToggle.onClick += ToggleShowPriorityInfo;
            disableLineAnimToggle.onClick += ToggleDisableLineAnim;
        }

        public void OnEnable()
        {
            UpdateState();
        }

        private void Update()
        {
            if (OverlayScreen.Instance.mode == MyOverlayModes.PortChannel.ID)
            {
                if (moreDetailParent.activeSelf == MyOverlayModes.PortChannel.IsActiveChannelStatus())
                {
                    UpdateState();
                }
            }

        }

        private void UpdateState()
        {
            if (OverlayScreen.Instance.mode == MyOverlayModes.PortChannel.ID)
            {
                lineToggleGroup.Select((int)MyOverlayModes.PortChannel.wiredPreviewMode);

                bool activeChannelStatus = MyOverlayModes.PortChannel.IsActiveChannelStatus();
                moreDetailParent.SetActiveNR(!activeChannelStatus);
                
                showPriorityInfoToggle.ChangeState(MyOverlayModes.PortChannel.showPriorityInfo? 1 : 0);
                disableLineAnimToggle.ChangeState(MyOverlayModes.PortChannel.disableLineAnim? 1 : 0);
                if (!activeChannelStatus)
                {
                    typeToggleGroup.Select((int)MyOverlayModes.PortChannel.buildingType);
                    showOnlyNameToggle.ChangeState(MyOverlayModes.PortChannel.showOnlyNullChannel ? 1 : 0);
                }
            }
        }

        /// <summary>
        /// 切换连线预览方式
        /// </summary>
        private void ToggleWiredPreviewMode(int i)
        {
            MyOverlayModes.PortChannel.wiredPreviewMode = (MyOverlayModes.PortChannel.WiredPreviewMode)i;
        }
        
        private void ToggleBuildingType(int i)
        {
            MyOverlayModes.PortChannel.buildingType = (BuildingType)i;
        }

        private void ToggleShowOnlyName()
        {
            if (MyOverlayModes.PortChannel.showOnlyNullChannel)
            {
                MyOverlayModes.PortChannel.showOnlyNullChannel = false;
                showOnlyNameToggle.ChangeState(0);
            }
            else
            {
                MyOverlayModes.PortChannel.showOnlyNullChannel = true;
                showOnlyNameToggle.ChangeState(1);
            }
        }
        
        private void ToggleGlobalChannel()
        {
            if (MyOverlayModes.PortChannel.showOnlyGlobalChannel)
            {
                MyOverlayModes.PortChannel.showOnlyGlobalChannel = false;
                showOnlyGlobalToggle.ChangeState(0);
            }
            else
            {
                MyOverlayModes.PortChannel.showOnlyGlobalChannel = true;
                showOnlyGlobalToggle.ChangeState(1);
            }
        }
        
        private void ToggleShowPriorityInfo()
        {
            if (MyOverlayModes.PortChannel.showPriorityInfo)
            {
                MyOverlayModes.PortChannel.showPriorityInfo = false;
                showPriorityInfoToggle.ChangeState(0);
            }
            else
            {
                MyOverlayModes.PortChannel.showPriorityInfo = true;
                showPriorityInfoToggle.ChangeState(1);
            }
        }
        
        private void ToggleDisableLineAnim()
        {
            if (MyOverlayModes.PortChannel.disableLineAnim)
            {
                MyOverlayModes.PortChannel.disableLineAnim = false;
                disableLineAnimToggle.ChangeState(0);
            }
            else
            {
                MyOverlayModes.PortChannel.disableLineAnim = true;
                disableLineAnimToggle.ChangeState(1);
            }
        }
    }
}