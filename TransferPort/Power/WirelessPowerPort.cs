namespace RsTransferPort {
    public class WirelessPowerPort :
        UtilityNetworkLink,
        IHaveUtilityNetworkMgr,
        ICircuitConnected {
        [MyCmpGet] public PortItem item;

        public bool IsVirtual { get; private set; }
        public int PowerCell => GetNetworkCell();
        public object VirtualCircuitKey { get; private set; }

        public IUtilityNetworkMgr GetNetworkManager() {
            return Game.Instance.electricalConduitSystem;
        }
        protected override void OnSpawn() {
            base.OnSpawn();
            item.OnEnterChannel += OnEnterChannel;
            item.OnExitChannel += OnExitChannel;
        }

        protected override void OnCleanUp() {
            if (!Util.IsNullOrDestroyed(item)) {
                item.OnEnterChannel -= OnEnterChannel;
                item.OnExitChannel -= OnExitChannel;
            }
            base.OnCleanUp();
        }

        protected void OnEnterChannel(SingleChannelController channel) {
            Disconnect();
            if (!channel.IsInvalid()) {
                VirtualCircuitKey = channel;
                Connect();
            }
            else {
                VirtualCircuitKey = null;
            }
        }
        protected void OnExitChannel(SingleChannelController channel) {
            Disconnect();
            VirtualCircuitKey = null;
        }

        protected override void OnDisconnect(int cell1, int cell2) {
            if (!(VirtualCircuitKey is SingleChannelController channelController)
                || channelController.IsInvalid()) {
                return;
            }
            var manager = GetNetworkManager();
            if (manager is UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem)
                electricalConduitSystem.RemoveSemiVirtualLink(cell1, VirtualCircuitKey);
        }

        protected override void OnConnect(int cell1, int cell2) {
            if (!(VirtualCircuitKey is SingleChannelController channelController)
                || channelController.IsInvalid()) {
                return;
            }
            var manager = GetNetworkManager();
            if (manager is UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem) {
                electricalConduitSystem.AddSemiVirtualLink(cell1, VirtualCircuitKey);
            }
        }

    }
}