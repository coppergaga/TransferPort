using System.Collections.Generic;

namespace RsTransferPort
{
    // public class TransferConduitManager : SingleManager<TransferConduitManager>
    // {
    //     private readonly Dictionary<ConduitChannelKey, PortalConduitChannel> channels =
    //         new();
    //
    //     private readonly Dictionary<KMonoBehaviour, ConduitChannelKey> objectChannelKey =
    //         new();
    //
    //     private PortalConduitChannel CreateOrGetChannel(ConduitChannelKey channelKey)
    //     {
    //         PortalConduitChannel conduitChannel = null;
    //
    //         if (!channels.TryGetValue(channelKey, out conduitChannel))
    //         {
    //             conduitChannel = new PortalConduitChannel();
    //             conduitChannel.Key = channelKey;
    //             channels.Add(channelKey, conduitChannel);
    //         }
    //
    //         return conduitChannel;
    //     }
    //
    //     public void Add(TransferConduit cop)
    //     {
    //         var channelKey = cop.GetConduitChannelKey();
    //         objectChannelKey[cop] = channelKey;
    //         CreateOrGetChannel(channelKey).Add(cop);
    //     }
    //
    //     public void Remove(TransferConduit cop)
    //     {
    //         ConduitChannelKey channelKey;
    //         if (objectChannelKey.TryGetValue(cop, out channelKey))
    //         {
    //             var channel = CreateOrGetChannel(channelKey);
    //             channel.Remove(cop);
    //             if (channel.totalCount == 0)
    //             {
    //                 channel.OnCleanup();
    //                 channels.Remove(channelKey);
    //             }
    //
    //             objectChannelKey.Remove(cop);
    //         }
    //     }
    //
    //     public bool Has(TransferConduit cop)
    //     {
    //         var channelKey = cop.GetConduitChannelKey();
    //         return channels.ContainsKey(channelKey);
    //     }
    //
    //     public ICollection<PortalConduitChannel> GetChannels()
    //     {
    //         return channels.Values;
    //     }
    //
    //     public ICollection<PortalConduitChannel> GetChannels(ConduitType conduitType, int worldId)
    //     {
    //         var collection = GetChannels();
    //         var portalConduitChannels = new List<PortalConduitChannel>();
    //
    //         foreach (var channel in collection)
    //         {
    //             var channelKey = channel.Key;
    //             if (channelKey.conduitType == conduitType && channelKey.worldId == worldId)
    //                 portalConduitChannels.Add(channel);
    //         }
    //
    //         return portalConduitChannels;
    //     }
    //
    //     public ICollection<PortalConduitChannel> GetChannels(int worldId)
    //     {
    //         var collection = GetChannels();
    //         var portalConduitChannels = new List<PortalConduitChannel>();
    //
    //         foreach (var channel in collection)
    //         {
    //             var channelKey = channel.Key;
    //             if (channelKey.worldId == worldId) portalConduitChannels.Add(channel);
    //         }
    //
    //         return portalConduitChannels;
    //     }
    //
    //     protected override void OnSpawn()
    //     {
    //         Game.Instance.gasConduitFlow.AddConduitUpdater(GasConduitUpdate, ConduitFlowPriority.Dispense);
    //         Game.Instance.liquidConduitFlow.AddConduitUpdater(LiquidConduitUpdate, ConduitFlowPriority.Dispense);
    //         Game.Instance.solidConduitFlow.AddConduitUpdater(SolidConduitUpdate, ConduitFlowPriority.Dispense);
    //     }
    //
    //     protected override void OnCleanUp()
    //     {
    //         if (Game.Instance != null)
    //         {
    //             Game.Instance.gasConduitFlow.RemoveConduitUpdater(GasConduitUpdate);
    //             Game.Instance.liquidConduitFlow.RemoveConduitUpdater(LiquidConduitUpdate);
    //             Game.Instance.solidConduitFlow.RemoveConduitUpdater(SolidConduitUpdate);
    //         }
    //
    //         channels.Clear();
    //         objectChannelKey.Clear();
    //     }
    //
    //     private void GasConduitUpdate(float dt)
    //     {
    //         ConduitUpdate(ConduitType.Gas, dt);
    //     }
    //
    //     private void LiquidConduitUpdate(float dt)
    //     {
    //         ConduitUpdate(ConduitType.Liquid, dt);
    //     }
    //
    //     private void SolidConduitUpdate(float dt)
    //     {
    //         ConduitUpdate(ConduitType.Solid, dt);
    //     }
    //
    //     private void ConduitUpdate(ConduitType conduitType, float dt)
    //     {
    //         var valueCollection = GetChannels();
    //         foreach (var conduitChannel in valueCollection)
    //             if (conduitChannel.conduitType == conduitType)
    //                 conduitChannel.ConduitUpdate(dt);
    //     }
    // }
}