using System.Collections.Generic;
using System.Linq;
using System.Text;
using RsLib;
using RsLib.Adapter;
using RsLib.Components;
using UnityEngine;
using UnityEngine.UI;
using OUI = STRINGS.UI;

namespace RsTransferPort {
    public class PortChannelSideScreen : SideScreenContent {
        [SerializeField] private MultiToggleAdapter detailLevelToggle;
        [SerializeField] private KInputTextFieldAdapter channelNameInputField;
        [SerializeField] private GameObject listContainer;
        [SerializeField] private LocTextAdapter headerLabel;
        [SerializeField] private MultiToggleAdapter openCandidateNameToggle;
        [SerializeField] private MultiToggleAdapter batchRenameToggle;
        [SerializeField] private MultiToggleAdapter globalToggle;
        [SerializeField] private PriorityBar priorityBar;

        [SerializeField] private LocTextAdapter warningLabel;

        [SerializeField] private RsHierarchyReferences row1Prefab;
        [SerializeField] private RsHierarchyReferences row2Prefab;
        [SerializeField] private LocTextAdapter infoLocTextPrefab;
        [SerializeField] private RsHierarchyReferences worldInfoPrefab;
        [SerializeField] private CandidateNameScreen candidateNameScreenPrefab;

        private PortItem target;
        private bool needRefresh = true;
        private RsInterval refreshInterval = new RsInterval(1);
        private bool isBatchMode = false;
        private bool isOpenCandidateName = false;
        private int detailLevel = 0;

        private RsHashUIPool<RsHierarchyReferences> rowPool2;
        private RsHashUIPool<LocTextAdapter> locTextPool;
        private RsHashUIPool<RsHierarchyReferences> worldInfoPool;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            activateOnSpawn = true;

            rowPool2 = new RsHashUIPool<RsHierarchyReferences>(row2Prefab);
            locTextPool = new RsHashUIPool<LocTextAdapter>(infoLocTextPrefab);
            worldInfoPool = new RsHashUIPool<RsHierarchyReferences>(worldInfoPrefab);

            detailLevelToggle.onClick = OnDetailLevelToggleClick;
            batchRenameToggle.onClick = OnBatchRenameClick;
            openCandidateNameToggle.onClick = OnCandidateNameToggleClick;
            globalToggle.onClick = OnGlobalToggleClick;

            channelNameInputField.onSelect.AddListener(OnChangeNameEditStart);
            channelNameInputField.onEndEdit.AddListener(OnChangeNameEditEnd);
            priorityBar.OnPriorityClick = OnPriorityClick;
            priorityBar.Help.OnToolTip = OnPriorityBarHelp;
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            headerLabel.SetText(STRINGS.UI.SIDESCREEN.RS_PORT_CHANNEL.CHANNEL_NAME);
            PortManager.Instance.OnChannelChange += HandleChannelChange;

            globalToggle.gameObject.SetActiveNR(DlcManager.IsExpansion1Active());
        }

        protected override void OnDeactivate() {
            PortManager.Instance.OnChannelChange -= HandleChannelChange;
        }

        protected void HandleChannelChange(PortItem item) {
            Refresh();
        }

        private void OnChangeNameEditStart(string text) {
            isEditing = true;
        }

        private void OnChangeNameEditEnd(string text) {
            isEditing = false;
            if (target != null) {
                ChangeName(text);
            }
            channelNameInputField.text = target.ChannelName;
        }

        protected void ChangeName(string str) {
            if (isBatchMode) {
                PortManager.Instance.BatchChange(GetController(target), str, target.IsGlobal);
                SetBatchRenameState(false);
                return;
            }
            if (!Util.IsNullOrDestroyed(target)) {
                target.CheckSetChannelName(str);
            }
        }

        protected void SetBatchRenameState(bool enable) {
            isBatchMode = enable;
            batchRenameToggle.ChangeState(isBatchMode ? 1 : 0);
            if (isBatchMode) {
                warningLabel.gameObject.SetActiveNR(true);
                warningLabel.SetTextNoRepeat(STRINGS.UI.SIDESCREEN.RS_PORT_CHANNEL.WARIN_BATCH_MODE);
            }
            else {
                warningLabel.gameObject.SetActiveNR(false);
            }
        }

        /// <summary>
        /// 详细等级
        /// </summary>
        private void OnDetailLevelToggleClick() {
            if (target != null) {
                detailLevel = ++detailLevel % 2;    // equals to detailLevel += 1; detailLevel %= 2;
                Refresh();
            }
        }

        private void OnBatchRenameClick() {
            SetBatchRenameState(!isBatchMode);
        }

        private void OnCandidateNameToggleClick() {
            isOpenCandidateName = !isOpenCandidateName;
            RefreshCandidateNameState();
        }

        private void OnGlobalToggleClick() {
            if (target != null) {
                if (isBatchMode) {
                    PortManager.Instance.BatchChange(GetController(target), target.ChannelName, !target.IsGlobal);
                    SetBatchRenameState(false);
                }
                else {
                    target.CheckSetGlobal(!target.IsGlobal);
                }
                globalToggle.ChangeState(target.IsGlobal ? 1 : 0);
                Refresh();
            }
        }

        private void RefreshCandidateNameState() {
            openCandidateNameToggle.ChangeState(isOpenCandidateName ? 1 : 0);
            if (DetailsScreen.Instance == null) { return; }
            if (isOpenCandidateName) {
                var sideScreen =
                    (CandidateNameScreen)DetailsScreen.Instance.SetSecondarySideScreen(
                        candidateNameScreenPrefab,
                        STRINGS.UI.SIDESCREEN.RS_CANDIDATE_NAME.TITLE);
                sideScreen.selected = ChangeName;
                sideScreen.SwitchCandidate(target.BuildingType);
            }
            else {
                DetailsScreen.Instance.ClearSecondarySideScreen();
            }
        }

        private string OnPriorityBarHelp() {
            if (Converter.IsUsePriority(target.BuildingType)
                && GetController(target) is TransferConduitChannel controller) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(STRINGS.UI.SIDESCREEN.RS_PORT_CHANNEL.PRIORITY_TOOLTIP);
                stringBuilder.AppendLine();
                for (int priority = 9; priority >= 0; priority--) {
                    PriorityChannelItemInfo senderPriorityInfo = controller.senderPriorityList.GetByPriority(priority);
                    PriorityChannelItemInfo receiverPriorityInfo = controller.receiverPriorityList.GetByPriority(priority);

                    int senderNum = senderPriorityInfo == null ? 0 : senderPriorityInfo.items.Count;
                    int receiverNum = receiverPriorityInfo == null ? 0 : receiverPriorityInfo.items.Count;

                    if (senderNum != 0 || receiverNum != 0) {
                        stringBuilder.AppendLine();
                        stringBuilder.AppendFormat(STRINGS.UI.SIDESCREEN.RS_PORT_CHANNEL.PRIORITY_LINE_INFO, priority, senderNum, receiverNum);
                    }
                }
                return stringBuilder.ToString();
            }
            return "无";
        }

        private void OnPriorityClick(int priority) {
            if (isBatchMode) {
                PortManager.Instance.BatchChangePriority(GetController(target), priority);
                SetBatchRenameState(false);
            }
            else {
                target.CheckSetPriority(priority);
            }
        }
        private void RefreshPriority() {
            if (Util.IsNullOrDestroyed(target)) { return; }
            if (Converter.IsUsePriority(target.BuildingType)
                && GetController(target) is TransferConduitChannel controller) {
                priorityBar.gameObject.SetActiveNR(true);
                priorityBar.SetAllStateCache(0);                //重置优先度
                //设置其它的优先度
                priorityBar.SetStateCacheRange(controller.senderPriorityList.AllPriority(), 2);
                priorityBar.SetStateCacheRange(controller.receiverPriorityList.AllPriority(), 2);
                priorityBar.SetStateCache(target.Priority, 1);  //设置目标优先度
                priorityBar.ApplyStateCache();                  //应用
            }
            else {
                priorityBar.gameObject.SetActiveNR(false);
            }
        }

        public override bool IsValidForTarget(GameObject target) {
            return !Util.IsNullOrDestroyed(target.GetComponent<PortItem>());
        }

        public override void SetTarget(GameObject new_target) {
            if (Util.IsNullOrDestroyed(new_target)) { return; }
            target = new_target.GetComponent<PortItem>();
            RefreshCandidateNameState();
            SetBatchRenameState(isBatchMode);
            Refresh();
        }

        public override void ClearTarget() {
            target = null;
            isBatchMode = false;
            DetailsScreen.Instance?.ClearSecondarySideScreen();
        }

        protected override void OnDisable() {
            base.OnDisable();
            isBatchMode = false;
        }

        public override void ScreenUpdate(bool topLevel) {
            base.ScreenUpdate(topLevel);
            if (refreshInterval.Update(Time.unscaledDeltaTime)) {
                needRefresh = true;
            }
            ImmediateRefresh();
        }

        private void Refresh() {
            needRefresh = true;
        }

        private void ImmediateRefresh() {
            if (!needRefresh) { return; }
            needRefresh = false;
            if (target == null) return;
            
            if (isEditing == false) {   //更新名称
                channelNameInputField.text = target.ChannelName;
            }
            RefreshPriority();

            batchRenameToggle.ChangeState(isBatchMode ? 1 : 0);
            detailLevelToggle.ChangeState(detailLevel);
            globalToggle.ChangeState(target.IsGlobal ? 1 : 0);

            ICollection<SingleChannelController> controllers =
                PortManager.Instance.GetChannels(target.BuildingType, target.WorldIdAG, true);

            detailLevelToggle.gameObject.SetActiveNR(true);

            rowPool2.RecordStart();
            locTextPool.RecordStart();
            worldInfoPool.RecordStart();

            foreach (SingleChannelController controller in controllers) {
                var localController = controller;
                RsHierarchyReferences row = rowPool2.GetFreeElement(localController, listContainer, true);
                row.transform.SetAsLastSibling();

                LocTextAdapter channelNameLocText = row.GetReference<LocTextAdapter>("ChannelName");
                channelNameLocText.SetTextNoRepeat(localController.DisplayChannelName);

                MultiToggle toggle = row.GetComponent<MultiToggle>();
                toggle.ChangeState(target.HasChannel(localController) ? 1 : 0);
                toggle.onClick = () => {
                    target.SetChannel(localController);
                };
                GameObject globalIcon = row.GetReference("GlobalIcon");
                globalIcon.SetActiveNR(localController.IsGlobal);

                LocTextAdapter num1 = row.GetReference<LocTextAdapter>("Num1");
                num1.gameObject.SetActiveNR(true);
                if (localController.BuildingType == BuildingType.Power) {
                    num1.SetTextNoRepeat("N:" + localController.senders.Count);
                }
                else {
                    num1.SetTextNoRepeat("S:" + localController.senders.Count);
                }

                LocTextAdapter num2 = row.GetReference<LocTextAdapter>("Num2");
                if (localController.BuildingType == BuildingType.Power) {
                    num2.gameObject.SetActiveNR(false);
                }
                else {
                    num2.gameObject.SetActiveNR(true);
                    num2.SetTextNoRepeat("R:" + localController.receivers.Count);
                }

                if (detailLevel == 1 && localController.BuildingType == BuildingType.Power && !localController.IsInvalid()) {
                    RefreshPowerInfo(row, localController);
                }

                if (detailLevel == 1 && localController.IsGlobal && DlcManager.IsContentSubscribed(DlcManager.EXPANSION1_ID)) {
                    RefreshWorldListInfo(row, localController);
                }
            }

            worldInfoPool.ClearNoRecordElement();
            locTextPool.ClearNoRecordElement();
            rowPool2.ClearNoRecordElement();
        }

        private void RefreshWorldListInfo(RsHierarchyReferences row, SingleChannelController controller) {
            GameObject infoList = row.GetReference("info");
            WorldContainer[] containers = controller.GetIncludeWorldContainer().ToArray();
            for (var i = 0; i < containers.Length; i++) {
                WorldContainer world = containers[i];
                RsHierarchyReferences element = worldInfoPool.GetFreeElement(infoList.GetHashCode() | i, infoList, true);
                element.transform.SetAsLastSibling();
                Image image = element.GetReference<Image>("WorldIcon");

                if (world.IsModuleInterior) {
                    image.sprite = global::Assets.GetSprite((HashedString)"icon_category_rocketry");
                    image.color = Color.white;
                    string name = world.GetComponent<Clustercraft>()?.Name ?? "unknown";
                    element.GetReference<LocTextAdapter>("Name").SetTextNoRepeat(name);
                }
                else {
                    image.sprite = Def.GetUISprite(world.GetComponent<ClusterGridEntity>()).first;
                    image.color = Def.GetUISprite(world.GetComponent<ClusterGridEntity>()).second;
                    string name = world.GetComponent<AsteroidGridEntity>()?.Name ?? "unknown";
                    element.GetReference<LocTextAdapter>("Name").SetTextNoRepeat(name);
                }
            }
        }

        private void RefreshPowerInfo(RsHierarchyReferences row, SingleChannelController controller) {
            GameObject infoList = row.GetReference("info");
            infoList.SetActiveNR(true);

            CircuitManager circuitManager = Game.Instance.circuitManager;
            ushort circuitID = circuitManager.GetVirtualCircuitID(controller);
            if (circuitID == ushort.MaxValue) { return; }

            LocTextAdapter label1 = locTextPool.GetFreeElement(infoList.GetHashCode() | 1, infoList, true);
            label1.transform.SetAsLastSibling();
            label1.SetTextNoRepeat(string.Format(
                (string)OUI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES,
                GameUtil.GetFormattedJoules(circuitManager.GetJoulesAvailableOnCircuit(circuitID))
            ));

            float watts = circuitManager.GetWattsGeneratedByCircuit(circuitID);
            float potentialWatts = circuitManager.GetPotentialWattsGeneratedByCircuit(circuitID);
            LocTextAdapter label2 = locTextPool.GetFreeElement(infoList.GetHashCode() | 2, infoList, true);
            label2.transform.SetAsLastSibling();
            string str = (double)watts != (double)potentialWatts
                ? string.Format("{0} / {1}", GameUtil.GetFormattedWattage(watts), GameUtil.GetFormattedWattage(potentialWatts))
                : GameUtil.GetFormattedWattage(watts);
            label2.SetTextNoRepeat(string.Format((string)OUI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED, str));

            LocTextAdapter label3 = locTextPool.GetFreeElement(infoList.GetHashCode() | 3, infoList, true);
            label3.transform.SetAsLastSibling();
            label3.SetTextNoRepeat(string.Format(
                (string)OUI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED,
                GameUtil.GetFormattedWattage(circuitManager.GetWattsUsedByCircuit(circuitID))
            ));

            LocTextAdapter label4 = locTextPool.GetFreeElement(infoList.GetHashCode() | 4, infoList, true);
            label4.transform.SetAsLastSibling();
            label4.SetTextNoRepeat(string.Format(
                (string)OUI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED,
                GameUtil.GetFormattedWattage(circuitManager.GetWattsNeededWhenActive(circuitID))
            ));

            LocTextAdapter label5 = locTextPool.GetFreeElement(infoList.GetHashCode() | 5, infoList, true);
            label5.transform.SetAsLastSibling();
            label5.SetTextNoRepeat(string.Format(
                (string)OUI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE,
                GameUtil.GetFormattedWattage(circuitManager.GetMaxSafeWattageForCircuit(circuitID))
            ));
        }

        private SingleChannelController GetController(PortItem item) {
            return PortManager.Instance.GetChannelController(item);
        }
    }
}