using System;
using KSerialization;
using UnityEngine;

namespace RsTransferPort {
    public interface ICustomLogicWrappable {
        Func<int> HandleReturnInt { get; set; }
        Func<float> HandleReturnFloat { get; set; }
        Action<int> HandleInParamInt { get; set; }
        Action<float> HandleInParamFloat { get; set; }
    }
    /// <summary>
    /// 传送端口中 固/液/气/辐射/电力端口的ViewModel类
    /// </summary>
    public class PortItem : KMonoBehaviour, ISaveLoadable, ICustomLogicWrappable {
        public delegate void PriorityChangeDelegate(PortItem target, int newPriority, int oldPriority);
        /// <summary>
        /// 连接状态
        /// </summary>
        private static StatusItem ConnectionStatusItem;

        /// <summary>
        /// 行星隔离模式
        /// </summary>
        private static StatusItem PlanetaryIsolationStatusItem;

        /// <summary>
        /// 全球互通模式
        /// </summary>
        private static StatusItem GlobalConnectivityStatusItem;

        public static Operational.Flag ConnectionFlag = new Operational.Flag("PortChannelChange", Operational.Flag.Type.Requirement);

        [MyCmpGet] private KSelectable kSelectable;
        [MyCmpGet] private Operational operational;

        [Serialize] protected string channelName = "";

        /// <summary>
        /// 是否为跨行星传送模式(宇宙互通模式)
        /// </summary>
        [Serialize] protected bool isGlobal = false;

        [Serialize] protected int priority = 5;

        [SerializeField] protected InOutType inOutType;

        [SerializeField] protected BuildingType buildingType;

        public event Action<SingleChannelController> OnEnterChannel;
        public event Action<SingleChannelController> OnExitChannel;
        public event PriorityChangeDelegate OnPriorityChange;

        public string ChannelName => channelName ?? "";

        public bool IsGlobal => isGlobal;

        public int Priority => priority;

        public PortChannelKey ChannelKey => new PortChannelKey(ChannelName, WorldIdAG, BuildingType);

        public int WorldIdAG => IsGlobal ? PortManager.GLOBAL_CHANNEL_WORLD_ID : this.GetMyWorldId();

        public string DisplayChannelName => string.IsNullOrEmpty(channelName)
            ? (string)STRINGS.UI.SIDESCREEN.RS_PORT_CHANNEL.CHANNEL_NULL
            : channelName;

        public InOutType InOutType {
            get => inOutType;
            set => inOutType = value;
        }

        public BuildingType BuildingType {
            get => buildingType;
            set => buildingType = value;
        }
        public Func<int> HandleReturnInt { get; set; }
        public Func<float> HandleReturnFloat { get; set; }
        public Action<int> HandleInParamInt { get; set; }
        public Action<float> HandleInParamFloat { get; set; }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.CopySettings, OnCopySettings);

            if (ConnectionStatusItem == null) {
                ConnectionStatusItem = new StatusItem("RsTransferPortChannelConnection", "BUILDING",
                    "unconnected_channel_icon", StatusItem.IconType.Custom, NotificationType.BadMinor, false,
                    MyOverlayModes.PortChannel.ID, true);
            }

            if (PlanetaryIsolationStatusItem == null) {
                PlanetaryIsolationStatusItem = new StatusItem("RsChannelPlanetaryIsolationModel", "BUILDING",
                    "planetary_isolation_icon", StatusItem.IconType.Custom, NotificationType.Neutral, false,
                    OverlayModes.None.ID, false);
            }

            if (GlobalConnectivityStatusItem == null) {
                GlobalConnectivityStatusItem = new StatusItem("RsChannelGlobalConnectivityModel", "BUILDING",
                    "global_connectivity_icon", StatusItem.IconType.Custom, NotificationType.Neutral, false,
                    OverlayModes.None.ID, false);
            }
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            if (channelName == null) { channelName = ""; }

            UIScheduler.Instance.Schedule("PortItemOnSpawn", 0f, SyncToPortManager, this);
        }

        protected override void OnCleanUp() {
            PortManager.Instance.Remove(this);
            PortManager.Instance.TriggerChannelChange(this);
        }

        protected void UpdateConnectionStatusItem() {
            bool b = string.IsNullOrEmpty(channelName);
            kSelectable.ToggleStatusItem(ConnectionStatusItem, b, this);
            operational.SetFlag(ConnectionFlag, !b);
        }


        protected void OnCopySettings(object data) {
            if (data is GameObject go && go != gameObject
                && go.GetComponent<PortItem>() is PortItem sourceItem
                && !Util.IsNullOrDestroyed(sourceItem)) {
                CheckSetChannelNameAndGlobal(sourceItem.ChannelName, sourceItem.IsGlobal, sourceItem.Priority);
            }
        }

        private static void SyncToPortManager(object data) {
            if (!Util.IsNullOrDestroyed(data) && data is PortItem self) {
                self.UpdateConnectionStatusItem();
                PortManager.Instance.Add(self);
                PortManager.Instance.TriggerChannelChange(self);
            }
        }

        /// <summary>
        /// 不重复触发事件
        /// </summary>
        public void CheckSetChannelName(string newName) {
            CheckSetChannelNameAndGlobal(newName, isGlobal, priority);
        }

        public void CheckSetGlobal(bool global) {
            CheckSetChannelNameAndGlobal(channelName, global, priority);
        }
        public void CheckSetPriority(int newPriority) {
            CheckSetChannelNameAndGlobal(channelName, isGlobal, newPriority);
        }

        public void CheckSetChannelNameAndGlobal(string newName, bool global, int newPriority) {
            int oldPriority = priority;
            bool isGlobalModeChange = isGlobal != global;
            bool isChannelNameChange = newName?.Trim() is string nn && !string.Equals(channelName, nn);
            bool isPriorityChange = newPriority != priority;

            if (isGlobalModeChange || isChannelNameChange) {
                // first remove item, then channelName = new name, last add item
                // because we get channel controller by the combined name, worldID and buildingType
                PortManager.Instance.Remove(this);
                isGlobal = global;
                channelName = newName;
                priority = newPriority;     // important!!! ensure data sync
                PortManager.Instance.Add(this);
                UpdateConnectionStatusItem();
            } else {
                priority = newPriority;     // important!!! ensure data sync
            }

            if (isGlobalModeChange || isChannelNameChange || isPriorityChange) {
                PortManager.Instance.TriggerChannelChange(this);
            }

            if (!isGlobalModeChange && !isChannelNameChange && isPriorityChange) {
                OnPriorityChange?.Invoke(this, priority, oldPriority);
            }
        }

        public void EnterChannelController(SingleChannelController controller) {
            if (kSelectable != null && DlcManager.IsExpansion1Active()) {
                kSelectable.ToggleStatusItem(PlanetaryIsolationStatusItem, !isGlobal);
                kSelectable.ToggleStatusItem(GlobalConnectivityStatusItem, isGlobal);
            }
            OnEnterChannel?.Invoke(controller);
        }

        public void ExitChannelController(SingleChannelController controller) {
            OnExitChannel?.Invoke(controller);
        }

        public void SetChannel(object target) {
            if (target is SingleChannelController channel) {
                CheckSetChannelName(channel.ChannelName);
            }
        }

        public bool HasChannel(object target) {
            if (target is SingleChannelController channel) {
                return channel.Contains(this);
            }
            return false;
        }
    }
}