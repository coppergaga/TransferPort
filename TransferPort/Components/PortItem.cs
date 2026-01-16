using System;
using KSerialization;
using UnityEngine;

namespace RsTransferPort {
    [Obsolete("use PortItem instead")]
    public class TransferPortChannel : PortItem { }

    /// <summary>
    /// 传送端口中 固/液/气/辐射/电力端口的ViewModel类
    /// </summary>
    public class PortItem : KMonoBehaviour, ISaveLoadable {
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

        [MyCmpReq] private KSelectable kSelectable;

        [Serialize] protected string channelName = "";

        /// <summary>
        /// 是否为跨行星传送模式(宇宙互通模式)
        /// </summary>
        [Serialize] protected bool isGlobal = false;

        [Serialize] protected int priority = 5;

        [SerializeField] protected InOutType inOutType;

        [SerializeField] protected BuildingType buildingType;

        /// <summary>
        /// 会在OnSpawn周期在触发
        /// </summary>
        // public event Action<string> OnChannelNameChanged;

        public event Action<string> OnChannelNameInitialized;
        public event Action<SingleChannelController> OnEnterChannel;
        public event Action<SingleChannelController> OnExitChannel;
        public event PriorityChangeDelegate OnPriorityChange;

        public string ChannelName {
            get => channelName ?? "";
        }

        public bool IsGlobal {
            get => isGlobal;
        }

        public int Priority {
            get => priority;
        }

        public PortChannelKey ChannelKey => new PortChannelKey(ChannelName, WorldIdAG, buildingType);

        public int WorldIdAG => IsGlobal ? PortManager.GLOBAL_CHANNEL_WORLD_ID : this.GetMyWorldId();

        public string DisplayChannelName {
            get => string.IsNullOrEmpty(channelName)
                ? STRINGS.UI.SIDESCREEN.RS_PORT_CHANNEL.CHANNEL_NULL.ToString()
                : channelName;
        }

        public InOutType InOutType {
            get => inOutType;
            set => inOutType = value;
        }

        public BuildingType BuildingType {
            get => buildingType;
            set => buildingType = value;
        }

        public SingleChannelController ChannelController => PortManager.Instance.GetChannelController(this);

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
            if (channelName == null) {
                channelName = "";
            }

            UpdateConnectionStatusItem();

            PortManager.Instance.Add(this);
            OnChannelNameInitialized?.Invoke(ChannelName);
            PortManager.Instance.TriggerChannelChange(this);
        }

        protected override void OnCleanUp() {
            PortManager.Instance.Remove(this);
            PortManager.Instance.TriggerChannelChange(this);
        }

        protected void UpdateConnectionStatusItem() {
            bool b = string.IsNullOrEmpty(channelName);
            GetComponent<KSelectable>().ToggleStatusItem(ConnectionStatusItem, b, this);
            GetComponent<Operational>().SetFlag(ConnectionFlag, !b);
        }


        protected void OnCopySettings(object data) {
            if (data is GameObject source && source != gameObject) {
                PortItem sourceChannelItem = source.GetComponent<PortItem>();
                if (sourceChannelItem != null) {
                    CheckSetChannelNameAndGlobal(sourceChannelItem.ChannelName, sourceChannelItem.isGlobal, sourceChannelItem.priority);
                }
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
            bool triggerAddOrRemove = false;
            bool triggerChannelChange = false;
            bool triggerPriorityChange = false;
            int oldPriority = priority;

            if (isGlobal != global) {
                isGlobal = global;
                triggerAddOrRemove = true;
                triggerChannelChange = true;
            }

            if (newName != null) {
                newName = newName.Trim();
                if (!System.Object.Equals(channelName, newName)) {
                    channelName = newName;
                    triggerAddOrRemove = true;
                    triggerChannelChange = true;
                }
            }

            if (newPriority != priority) {
                priority = newPriority;
                triggerChannelChange = true;
                triggerPriorityChange = true;
            }

            if (triggerAddOrRemove) {
                PortManager.Instance.Remove(this);
                PortManager.Instance.Add(this);
                UpdateConnectionStatusItem();
            }

            if (triggerChannelChange) {
                PortManager.Instance.TriggerChannelChange(this);
            }

            if (!triggerAddOrRemove && triggerPriorityChange) {
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
                return channel?.Contains(this) ?? false;
            }

            return false;
        }

    }
}