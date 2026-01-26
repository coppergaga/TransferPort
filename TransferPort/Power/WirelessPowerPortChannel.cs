namespace RsTransferPort {
    public class WirelessPowerPortChannel : SingleChannelController {
        public WirelessPowerPortChannel(BuildingType buildingType, string channelName, int worldIdAG) 
            : base(buildingType, channelName, worldIdAG) {
        }

        protected override void OnAdd(PortItem item) {
            base.OnAdd(item);
            UIScheduler.Instance.Schedule("WirelessPowerPortChannelOnAdd", 0f, (_) => { if (!Util.IsNullOrDestroyed(this) && !Util.IsNullOrDestroyed(item)) { item.EnterChannelController(this); } });
        }
    }
}