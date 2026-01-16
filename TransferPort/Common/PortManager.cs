using System;
using System.Collections.Generic;
using System.Linq;

namespace RsTransferPort {
    public class PortManager : SingleManager<PortManager> {
        public const int GLOBAL_CHANNEL_WORLD_ID = -1;

        private readonly Dictionary<PortItem, PortChannelKey> allPort = new Dictionary<PortItem, PortChannelKey>();

        private readonly HashSet<SingleChannelController> allChannel = new HashSet<SingleChannelController>();

        private readonly Dictionary<BuildingType, Dictionary<PortChannelKey, SingleChannelController>> classifyChannels
            = new Dictionary<BuildingType, Dictionary<PortChannelKey, SingleChannelController>>() {
                [BuildingType.Gas] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.Liquid] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.Solid] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.Power] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.Logic] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.HEP] = new Dictionary<PortChannelKey, SingleChannelController>(),
            };

        public event Action<PortItem> OnChannelChange;

        /// <summary>
        /// 开启跨行星频道
        /// </summary>
        // public bool EnableGlobalChannel { get; private set; }

        private int GetWorldIdAG(PortItem portChannelItem) {
            return portChannelItem.IsGlobal ? GLOBAL_CHANNEL_WORLD_ID : portChannelItem.GetMyWorldId();
        }

        public void Add(PortItem portChannelItem) {
            if (Contains(portChannelItem)) {
                //throw new Exception("Repeat addition PortChannelItem");
                return;
            }

            PortChannelKey channelKey =
                new PortChannelKey(portChannelItem.ChannelName, portChannelItem.WorldIdAG, portChannelItem.BuildingType);

            allPort.Add(portChannelItem, channelKey);
            var channels = classifyChannels[channelKey.buildingType];
            if (!channels.TryGetValue(channelKey, out SingleChannelController sc)) {
                sc = CreateSingleChannelController(channelKey.buildingType, channelKey.name, channelKey.worldId);
                sc.OnSpawn();
                channels.Add(channelKey, sc);
                allChannel.Add(sc);
            }

            if (!sc.Contains(portChannelItem)) {
                //这里进行添加
                sc.Add(portChannelItem);
                portChannelItem.EnterChannelController(sc);
            }

            //检查是否已经有空频道，无则创建
            PortChannelKey nullChannelKey = new PortChannelKey("", channelKey.worldId, channelKey.buildingType);
            if (!channels.ContainsKey(nullChannelKey)) {
                channels.Add(nullChannelKey, CreateSingleChannelController(channelKey.buildingType, "", channelKey.worldId));
            }

        }

        public bool Contains(PortItem portChannelItem) {
            return allPort.ContainsKey(portChannelItem);
        }

        public void Remove(PortItem portChannelItem) {
            if (!Contains(portChannelItem))
                return;

            PortChannelKey channelKey = allPort[portChannelItem];

            var channels = classifyChannels[channelKey.buildingType];

            if (channels.TryGetValue(channelKey, out SingleChannelController sc)) {
                sc.Remove(portChannelItem);
                portChannelItem.ExitChannelController(sc);
                if (!String.IsNullOrEmpty(sc.ChannelName) && sc.Total == 0) {
                    sc.OnCleanUp();
                    channels.Remove(channelKey);
                    allChannel.Remove(sc);
                }
            }

            allPort.Remove(portChannelItem);
        }

        public void TriggerChannelChange(PortItem target) {
            OnChannelChange?.Invoke(target);
        }

        public void BatchChange(SingleChannelController controller, string newName, bool global) {
            if (controller == null || (controller.ChannelName == newName && controller.IsGlobal == global)) {
                return;
            }

            foreach (PortItem channel in controller.all.ToList()) {
                channel.CheckSetChannelNameAndGlobal(newName, global, channel.Priority);
            }
        }

        public void BatchChangePriority(SingleChannelController controller, int priority) {
            if (controller == null) {
                return;
            }

            foreach (PortItem channel in controller.all.ToList()) {
                channel.CheckSetPriority(priority);
            }
        }

        private SingleChannelController CreateSingleChannelController(BuildingType type, string channelName, int worldId) {
            switch (type) {
                case BuildingType.Gas:
                case BuildingType.Liquid:
                case BuildingType.Solid:
                    return new TransferConduitChannel(type, channelName, worldId);
                case BuildingType.Power:
                    return new WirelessPowerPortChannel(type, channelName, worldId);
                case BuildingType.Logic:
                    return new WirelessLogicPortChannel(type, channelName, worldId);
                case BuildingType.HEP:
                    return new RadiantParticlesTransferChannel(type, channelName, worldId);
                default:
                    throw new Exception("Undefined " + type);
            }
        }

        /// <summary>
        /// 返回通道受跨行星影响
        /// </summary>
        /// <param name="worldId">启用GlobalChannel时会无效</param>
        public ICollection<SingleChannelController> GetChannels(BuildingType buildingType, int worldId, bool sort = false) {
            List<SingleChannelController> list = classifyChannels[buildingType].Values.Where((sc) => sc.WorldIdAG == worldId).ToList();

            if (sort && list.Count > 0) {
                list.Sort();
                if (!String.IsNullOrEmpty(list[0].ChannelName)) {
                    int index = list.FindIndex((sc) => String.IsNullOrEmpty(sc.ChannelName));
                    if (index != -1) {
                        (list[index], list[0]) = (list[0], list[index]);
                    }
                }
            }
            return list;
        }

        public ICollection<SingleChannelController> GetChannels(BuildingType buildingType) {
            return classifyChannels[buildingType].Values;
        }

        public ICollection<SingleChannelController> GetChannels() {
            return allChannel;
        }

        public ICollection<PortItem> GetAllPort() {
            return allPort.Keys;
        }

        public SingleChannelController GetChannelController(PortItem item) {
            if (!allPort.TryGetValue(item, out PortChannelKey channelKey)) {
                return null;
            }

            BuildingType buildingType = channelKey.buildingType;
            classifyChannels[buildingType].TryGetValue(channelKey, out SingleChannelController controller);
            return controller;
        }

        public SingleChannelController GetChannelControllerAG(BuildingType buildingType, int worldIdAG, string name) {
            PortChannelKey channelKey = new PortChannelKey(name, worldIdAG, buildingType);
            classifyChannels[buildingType].TryGetValue(channelKey, out SingleChannelController controller);
            return controller;
        }

        protected override void OnSpawn() {
            Game.Instance.gasConduitFlow.AddConduitUpdater(GasConduitUpdate, ConduitFlowPriority.Dispense);
            Game.Instance.liquidConduitFlow.AddConduitUpdater(LiquidConduitUpdate, ConduitFlowPriority.Dispense);
            Game.Instance.solidConduitFlow.AddConduitUpdater(SolidConduitUpdate, ConduitFlowPriority.Dispense);
        }

        protected override void OnCleanUp() {
            if (Game.Instance != null) {
                Game.Instance.gasConduitFlow.RemoveConduitUpdater(GasConduitUpdate);
                Game.Instance.liquidConduitFlow.RemoveConduitUpdater(LiquidConduitUpdate);
                Game.Instance.solidConduitFlow.RemoveConduitUpdater(SolidConduitUpdate);
            }
            // channels.Clear();
            // objectChannelKey.Clear();
        }

        private void GasConduitUpdate(float dt) {
            foreach (SingleChannelController controller in GetChannels(BuildingType.Gas)) {
                ((TransferConduitChannel)controller).ConduitUpdate(dt);
            }
        }

        private void LiquidConduitUpdate(float dt) {
            foreach (SingleChannelController controller in GetChannels(BuildingType.Liquid)) {
                ((TransferConduitChannel)controller).ConduitUpdate(dt);
            }
        }

        private void SolidConduitUpdate(float dt) {
            foreach (SingleChannelController controller in GetChannels(BuildingType.Solid)) {
                ((TransferConduitChannel)controller).ConduitUpdate(dt);
            }
        }
    }
}