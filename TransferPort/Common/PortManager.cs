using System;
using System.Collections.Generic;
using System.Linq;

namespace RsTransferPort
{
    public class PortManager : SingleManager<PortManager>
    {
        public const int GLOBAL_CHANNEL_WORLD_ID = -1;

        private Dictionary<TransferPortChannel, PortChannelKey> allPort = new Dictionary<TransferPortChannel, PortChannelKey>();

        private HashSet<SingleChannelController> allChannel = new HashSet<SingleChannelController>();

        private Dictionary<BuildingType, Dictionary<PortChannelKey, SingleChannelController>> classifyChannels
            = new Dictionary<BuildingType, Dictionary<PortChannelKey, SingleChannelController>>()
            {
                [BuildingType.Gas] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.Liquid] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.Solid] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.Power] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.Logic] = new Dictionary<PortChannelKey, SingleChannelController>(),
                [BuildingType.HEP] = new Dictionary<PortChannelKey, SingleChannelController>(),
            };

        // private HashSet<TransferPortCenter> centers = new();


        public event Action<TransferPortChannel> OnChannelChange;

        /// <summary>
        /// 开启跨行星频道
        /// </summary>
        // public bool EnableGlobalChannel { get; private set; }

        private int GetWorldIdAG(TransferPortChannel portChannelItem)
        {
            return portChannelItem.IsGlobal ? GLOBAL_CHANNEL_WORLD_ID : portChannelItem.GetMyWorldId();
        }

        public void Add(TransferPortChannel portChannelItem)
        {
            if (allPort.ContainsKey(portChannelItem))
            {
                throw new Exception("Repeat addition PortChannelItem");
            }

            PortChannelKey channelKey =
                new PortChannelKey(portChannelItem.ChannelName, portChannelItem.WorldIdAG, portChannelItem
                    .BuildingType);
            
            allPort.Add(portChannelItem, channelKey);
            var channels = classifyChannels[channelKey.buildingType];
            SingleChannelController sc;
            if (!channels.TryGetValue(channelKey, out sc))
            {
                sc = CreateSingleChannelController(channelKey.buildingType, channelKey.name, channelKey.worldId);
                sc.OnSpawn();
                channels.Add(channelKey, sc);
                allChannel.Add(sc);
            }

            if (!sc.Contains(portChannelItem))
            {
                //这里进行添加
                sc.Add(portChannelItem);
                portChannelItem.EnterChannelController(sc);
            }
            
            //检查是否已经有空频道，无则创建
            PortChannelKey nullChannelKey = new PortChannelKey("", channelKey.worldId, channelKey.buildingType);
            if (!channels.ContainsKey(nullChannelKey))
            {
                channels.Add(nullChannelKey, CreateSingleChannelController(channelKey.buildingType, "", channelKey.worldId));
            }

        }

        public bool Contains(TransferPortChannel portChannelItem)
        {
            return allPort.ContainsKey(portChannelItem);
        }

        public void Remove(TransferPortChannel portChannelItem)
        {
            if (!allPort.ContainsKey(portChannelItem))
                return;

            PortChannelKey channelKey = allPort[portChannelItem];

            var channels2
                = classifyChannels[channelKey.buildingType];

            SingleChannelController channel;
            if (channels2.TryGetValue(channelKey, out channel))
            {
                channel.Remove(portChannelItem);
                portChannelItem.ExitChannelController(channel);
                if (!String.IsNullOrEmpty(channel.ChannelName) && channel.Total == 0)
                {
                    channel.OnCleanUp();
                    channels2.Remove(channelKey);
                    allChannel.Remove(channel);
                }
            }

            allPort.Remove(portChannelItem);
        }

        public void TriggerChannelChange(TransferPortChannel target)
        {
            OnChannelChange?.Invoke(target);
        }

        // public void AddCenter(TransferPortCenter center)
        // {
        //     centers.Add(center);
        //         // DoEnableGlobalChannel(true);
        //     // if (EnableGlobalChannel == false)
        //     // {
        //     // }
        // }

        // public void RemoveCenter(TransferPortCenter center)
        // {
        //     centers.Remove(center);
        // }
        
        public void BatchChange(SingleChannelController controller, string newName, bool global)
        {
            if (controller == null || (controller.ChannelName == newName && controller.IsGlobal == global))
            {
                return;
            }

            List<TransferPortChannel> all = new List<TransferPortChannel>(controller.all);
            foreach (TransferPortChannel channel in all)
            {
                channel.CheckSetChannelNameAndGlobal(newName, global, channel.Priority);
            }
        }
        
        public void BatchChangePriority(SingleChannelController controller, int priority)
        {
            if (controller == null)
            {
                return;
            }

            List<TransferPortChannel> all = new List<TransferPortChannel>(controller.all);
            foreach (TransferPortChannel channel in all)
            {
                channel.CheckSetPriority(priority);
            }
        }
        
        
        // public void BatchChangeName(BuildingType buildingType, int wordId, string oldName, string newName)
        // {
        //     if (oldName == newName)
        //     {
        //         return;
        //     }
        //     SingleChannelController controller = GetChannelControllerAG(buildingType, wordId, oldName);
        //     if (controller == null)
        //         return;
        //
        //     BatchChangeName(controller, newName);
        // }

        // private void DoEnableGlobalChannel(bool enable)
        // {
        //     if (EnableGlobalChannel == enable)
        //         return;
        //
        //     //复制
        //     List<TransferPortChannel> channelItems = new List<TransferPortChannel>(allPort.Keys);
        //     //先移除
        //     foreach (TransferPortChannel channelItem in channelItems)
        //     {
        //         Remove(channelItem);
        //     }
        //     //移除空频道
        //     foreach (var c in classifyChannels.Values)
        //     {
        //         c.Clear();
        //     }
        //
        //     EnableGlobalChannel = enable;
        //
        //     foreach (TransferPortChannel channelItem in channelItems)
        //     {
        //         Add(channelItem);
        //     }
        // }

        private SingleChannelController CreateSingleChannelController(BuildingType type, string channelName,
            int worldId)
        {
            switch (type)
            {
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
        /// <param name="buildingType"></param>
        /// <param name="worldId">启用GlobalChannel时会无效</param>
        /// <returns></returns>
        public ICollection<SingleChannelController> GetChannels(BuildingType buildingType, int worldId, bool sort = false)
        {
            List<SingleChannelController> list = classifyChannels[buildingType].Values
                .Where((o) =>  o.WorldIdAG == worldId).ToList();
            
            if (sort && list.Count > 0)
            {
                list.Sort();
                if (!String.IsNullOrEmpty(list[0].ChannelName) )
                {
                    int index = list.FindIndex((o)=> String.IsNullOrEmpty(o.ChannelName));
                    if (index != -1)
                    {
                        var temp = list[0];
                        list[0] = list[index];
                        list[index] = temp;
                    }
                }
            }
            return list;
        }

        public ICollection<SingleChannelController> GetChannels(BuildingType buildingType)
        {
            return classifyChannels[buildingType].Values;
        }

        public ICollection<SingleChannelController> GetChannels()
        {
            return allChannel;
        }

        public ICollection<TransferPortChannel> GetAllPort()
        {
            return allPort.Keys;
        }

        public SingleChannelController GetChannelController(TransferPortChannel item)
        {
            PortChannelKey channelKey;
            if (!allPort.TryGetValue(item, out channelKey))
            {
                return null;
            }

            BuildingType buildingType = channelKey.buildingType;
            SingleChannelController controller;
            classifyChannels[buildingType].TryGetValue(channelKey, out controller);
            return controller;
        }
        
        public SingleChannelController GetChannelControllerAG(BuildingType buildingType, int worldIdAG, string name)
        {
            PortChannelKey channelKey = new PortChannelKey(name, worldIdAG, buildingType);
            SingleChannelController controller;
            classifyChannels[buildingType].TryGetValue(channelKey, out controller);
            return controller;
        }

        protected override void OnSpawn()
        {
            Game.Instance.gasConduitFlow.AddConduitUpdater(GasConduitUpdate, ConduitFlowPriority.Dispense);
            Game.Instance.liquidConduitFlow.AddConduitUpdater(LiquidConduitUpdate, ConduitFlowPriority.Dispense);
            Game.Instance.solidConduitFlow.AddConduitUpdater(SolidConduitUpdate, ConduitFlowPriority.Dispense);
        }

        protected override void OnCleanUp()
        {
            if (Game.Instance != null)
            {
                Game.Instance.gasConduitFlow.RemoveConduitUpdater(GasConduitUpdate);
                Game.Instance.liquidConduitFlow.RemoveConduitUpdater(LiquidConduitUpdate);
                Game.Instance.solidConduitFlow.RemoveConduitUpdater(SolidConduitUpdate);
            }
            // channels.Clear();
            // objectChannelKey.Clear();
        }

        private void GasConduitUpdate(float dt)
        {
            foreach (SingleChannelController controller in GetChannels(BuildingType.Gas))
            {
                ((TransferConduitChannel) controller).ConduitUpdate(dt);
            }
        }

        private void LiquidConduitUpdate(float dt)
        {
            foreach (SingleChannelController controller in GetChannels(BuildingType.Liquid))
            {
                ((TransferConduitChannel) controller).ConduitUpdate(dt);
            }
        }

        private void SolidConduitUpdate(float dt)
        {
            foreach (SingleChannelController controller in GetChannels(BuildingType.Solid))
            {
                ((TransferConduitChannel) controller).ConduitUpdate(dt);
            }
        }
        
        
    }
}