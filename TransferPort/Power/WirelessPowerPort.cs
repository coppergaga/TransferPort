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
            Subscribe((int)MyGameHashes.OnPortItemEnterChannel, OnPortItemEnterChannelDelegate);
            Subscribe((int)MyGameHashes.OnPortItemExistChannel, OnPortItemExistChannelDelegate);
        }

        protected override void OnCleanUp() {
            Unsubscribe((int)MyGameHashes.OnPortItemEnterChannel, OnPortItemEnterChannelDelegate);
            Unsubscribe((int)MyGameHashes.OnPortItemExistChannel, OnPortItemExistChannelDelegate);
            base.OnCleanUp();
        }

        protected void OnEnterChannel(object data) {
            if (!(data is SingleChannelController channel)) { return; }
            Disconnect();
            if (!channel.IsInvalid()) {
                VirtualCircuitKey = channel;
                Connect();
            }
            else {
                VirtualCircuitKey = null;
            }
        }
        protected void OnExitChannel(object data) {
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

        private static readonly EventSystem.IntraObjectHandler<WirelessPowerPort> OnPortItemEnterChannelDelegate =
            new EventSystem.IntraObjectHandler<WirelessPowerPort>(delegate (WirelessPowerPort cmp, object data) { cmp.OnEnterChannel(data); });
        private static readonly EventSystem.IntraObjectHandler<WirelessPowerPort> OnPortItemExistChannelDelegate =
            new EventSystem.IntraObjectHandler<WirelessPowerPort>(delegate (WirelessPowerPort cmp, object data) { cmp.OnExitChannel(data); });
    }
}