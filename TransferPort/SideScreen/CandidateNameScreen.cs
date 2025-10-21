using System;
using System.Collections.Generic;
using RsLib;
using RsLib.Adapter;
using RsLib.Components;
using UnityEngine;

namespace RsTransferPort
{
    public class CandidateNameScreen : KScreen
    {
        [SerializeField] protected RsHierarchyReferences rowPrefab;
        [SerializeField] protected GameObject listContainer;
        [SerializeField] protected MultiToggleAdapter supplyToggle;
        [SerializeField] protected MultiToggleAdapter temperatureToggle;

        private BuildingType currentBuildingType;

        public Action<string> selected;

        private RsHashUIPool<RsHierarchyReferences> rowPool;

        private int m_supplyState = 0;

        /// <summary>
        /// 0无 1供应 2 回收
        /// </summary>
        private int supplyState
        {
            get => m_supplyState;
            set
            {
                m_supplyState = value % 3;
                if (!RsUtil.IsNullOrDestroyed(supplyToggle))
                {
                    supplyToggle.ChangeState(m_supplyState);
                    supplyToggle.FindOrAddComponent<ToolTip>().toolTip =
                        Strings.Get("STRINGS.UI.SIDESCREEN.RS_CANDIDATE_NAME.SUPPLY_STATE_" + m_temperatureState);
                }
               
            }
        }

        private int m_temperatureState;
        /// <summary>
        /// 0无 1低温 2高温
        /// </summary>
        private int temperatureState
        {
            get => m_temperatureState;
            set
            {
                m_temperatureState = value % 3;
                if (!RsUtil.IsNullOrDestroyed(supplyToggle))
                {
                    temperatureToggle.ChangeState(m_temperatureState);
                    temperatureToggle.FindOrAddComponent<ToolTip>().toolTip =
                        Strings.Get("STRINGS.UI.SIDESCREEN.RS_CANDIDATE_NAME.TEMPERATURE_STATE_" + m_temperatureState);
                }
            }
        }

        private static bool initialized = false;
        private static Dictionary<BuildingType, string[]> candidateNameMap = new Dictionary<BuildingType, string[]>()
        {
            [BuildingType.Gas] = new string[]
            {
                "GAS_0",
                "GAS_1",
                "GAS_2",
                "GAS_3",
                "GAS_4",
                "GAS_5",
                "GAS_6",
                "GAS_7",
            },
            [BuildingType.Liquid] = new string[]
            {
                "LIQUID_0",
                "LIQUID_1",
                "LIQUID_2",
                "LIQUID_3",
                "LIQUID_4",
                "LIQUID_5",
                "LIQUID_6",
                "LIQUID_7",
                "LIQUID_8",
                "LIQUID_9",
                "LIQUID_10",
                "LIQUID_11",
                "LIQUID_12",
            },
            [BuildingType.Solid] = new string[]
            {
                "SOLID_0",
                "SOLID_1",
                "SOLID_2",
                "SOLID_3",
                "SOLID_4",
                "SOLID_5",
                "SOLID_6",
                "SOLID_7",
                "SOLID_8",
                "SOLID_9",
            },
            [BuildingType.Power] = new string[]
            {
                "POWER_0",
                "POWER_1",
                "POWER_2",
                "POWER_3",
                "POWER_4",
                "POWER_5",
            },
            [BuildingType.Logic] = new string[]
            {
                "LOGIC_0",
                "LOGIC_1",
                "LOGIC_2",
                "LOGIC_3",
                "LOGIC_4"
            },
            [BuildingType.HEP] = new string[]
            {
                "HEP_0",
                "HEP_1",
                "HEP_2",
                "HEP_3",
                "HEP_4",
                "HEP_5",
            },
        };


        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            rowPool = new RsHashUIPool<RsHierarchyReferences>(rowPrefab);
            if (supplyToggle == null || temperatureToggle == null)
            {
                Debug.LogWarning("supplyToggle or temperatureToggle is null");
                return;
            }

            supplyState = 0;
            supplyToggle.onClick = delegate
            {
                supplyState = ++supplyState % 3;
                supplyToggle.ChangeState(supplyState);
                Refresh();
            };

            temperatureState = 0;
            temperatureToggle.onClick = delegate
            {
                temperatureState = ++temperatureState % 3;
                temperatureToggle.ChangeState(temperatureState);
                Refresh();
            };

            if (!initialized)
            {
                foreach (var keyValuePair in candidateNameMap)
                {
                    string[] names = keyValuePair.Value;
                    for (var i = 0; i < names.Length; i++)
                    {
                        names[i] = Strings.Get("STRINGS.UI.SIDESCREEN.RS_CANDIDATE_NAME.LABELS." + names[i]);
                    }
                }
                initialized = true;
            }
            
        }

        public void SwitchCandidate(BuildingType buildingType)
        {
            if (currentBuildingType != buildingType)
            {
                currentBuildingType = buildingType;
                temperatureState = 0;
                supplyState = 0;
                Refresh();
            }
        }

        private void Refresh()
        {
            
            
            if (currentBuildingType == BuildingType.None || !candidateNameMap.ContainsKey(currentBuildingType))
            {
                rowPool.ClearAll();
                return;
            }

            string[] candidateName = candidateNameMap[currentBuildingType];

            rowPool.RecordStart();
            foreach (string sName in candidateName)
            {
                var name = sName;
                //生成名称
                if (supplyState == 1)
                {
                    name = name + STRINGS.UI.SIDESCREEN.RS_CANDIDATE_NAME.S_NAMES.SUPPLY;
                }
                else if (supplyState == 2)
                {
                    name = name + STRINGS.UI.SIDESCREEN.RS_CANDIDATE_NAME.S_NAMES.RECYCLE;
                }

                if (temperatureState == 1)
                {
                    name = STRINGS.UI.SIDESCREEN.RS_CANDIDATE_NAME.S_NAMES.LOW_TEMPERATURE + name;
                }
                else if (temperatureState == 2)
                {
                    name = STRINGS.UI.SIDESCREEN.RS_CANDIDATE_NAME.S_NAMES.HIGH_TEMPERATURE + name;
                }

                RsHierarchyReferences references = rowPool.GetFreeElement(name, listContainer, true);
                references.transform.SetAsLastSibling();

                LocTextAdapter textAdapter = references.GetReference<LocTextAdapter>("ChannelName");
                textAdapter.SetTextNoRepeat(name);

                MultiToggle toggle = references.GetComponent<MultiToggle>();
                toggle.onClick = () => OnRowClick(name);
            }

            rowPool.ClearNoRecordElement();
        }

        private void OnRowClick(string name)
        {
            selected?.Invoke(name);
        }

        
    }
}