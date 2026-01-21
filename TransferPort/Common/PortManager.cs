using System;
using System.Collections.Generic;
using System.Linq;

namespace RsTransferPort {
    public class PortManager : SingleManager<PortManager> {
        public const int GLOBAL_CHANNEL_WORLD_ID = -1;

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

        public void Add(PortItem item) {
            var buildingType = item.BuildingType;
            var channelName = item.ChannelName;
            var worldID = item.WorldIdAG;

            var channelKey = item.ChannelKey;
            var channels = classifyChannels[buildingType];
            if (!channels.TryGetValue(channelKey, out SingleChannelController sc)) {
                sc = CreateSingleChannelController(buildingType, channelName, worldID);
                sc.OnSpawn();
                channels.Add(channelKey, sc);
            }

            if (!sc.Contains(item)) {
                sc.Add(item);
                item.EnterChannelController(sc);
            }

            //检查是否已经有空频道，无则创建
            PortChannelKey nullChannelKey = new PortChannelKey("", worldID, buildingType);
            if (!channels.ContainsKey(nullChannelKey)) {
                channels.Add(nullChannelKey, CreateSingleChannelController(buildingType, "", worldID));
            }
        }

        public void Remove(PortItem item) {
            var channelKey = item.ChannelKey;
            var channels = classifyChannels[item.BuildingType];
            if (channels.TryGetValue(channelKey, out SingleChannelController sc)) {
                sc.Remove(item);
                item.ExitChannelController(sc);
                if (sc.IsNeedClean) {
                    sc.OnCleanUp();
                    channels.Remove(channelKey);
                }
            }
        }

        public void TriggerChannelChange(PortItem target) {
            OnChannelChange?.Invoke(target);
        }

        public void BatchChange(SingleChannelController controller, string newName, bool global) {
            if (controller == null || (controller.ChannelName == newName && controller.IsGlobal == global)) {
                return;
            }

            foreach (PortItem item in controller.all.ToList()) {
                item.CheckSetChannelNameAndGlobal(newName, global, item.Priority);
            }
        }

        public void BatchChangePriority(SingleChannelController controller, int priority) {
            if (controller == null) {
                return;
            }

            foreach (PortItem item in controller.all.ToList()) {
                item.CheckSetPriority(priority);
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
                if (!list[0].IsInvalid()) {
                    int index = list.FindIndex((sc) => sc.IsInvalid());
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
            var ret = new List<SingleChannelController>();
            foreach (var channel in classifyChannels.Values) {
                ret.AddRange(channel.Values);
            }
            return ret;
        }

        public ICollection<PortItem> GetAllPort() {
            List<PortItem> ret = new List<PortItem>();
            foreach (var channel in classifyChannels.Values) {
                foreach (var controller in channel.Values) {
                    ret.AddRange(controller.all);
                }
            }
            return ret;
        }

        public SingleChannelController GetChannelController(PortItem item) {
            classifyChannels[item.BuildingType].TryGetValue(item.ChannelKey, out SingleChannelController controller);
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