namespace RsTransferPort
{
    public class WirelessPowerPort :
        UtilityNetworkLink,
        IHaveUtilityNetworkMgr,
        ICircuitConnected
    {
        [MyCmpGet] public PortItem channelItem;

        public object VirtualCircuitKey { get; private set; }

        public bool IsVirtual { get; private set; }
        public int PowerCell => GetNetworkCell();

        public IUtilityNetworkMgr GetNetworkManager()
        {
            return Game.Instance.electricalConduitSystem;
        }
        
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            channelItem.OnEnterChannel += OnEnterChannel;
            channelItem.OnExitChannel += OnExitChannel;
        }

        protected void OnEnterChannel(SingleChannelController channel)
        {
            Disconnect();
            if (!channel.IsInvalid())
            {
                VirtualCircuitKey = channel;
                Connect();
            }
            else
            {
                VirtualCircuitKey = null;
            }
        }
        protected void OnExitChannel(SingleChannelController channel)
        {
            Disconnect();
            VirtualCircuitKey = null;
        }

        protected override void OnDisconnect(int cell1, int cell2)
        {
            SingleChannelController channelController = VirtualCircuitKey as SingleChannelController;
            if (channelController == null || channelController.IsInvalid() )
            {
                return;
            }
            var manager = GetNetworkManager();
            if (manager is UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem)
                electricalConduitSystem.RemoveSemiVirtualLink(cell1, VirtualCircuitKey);
        }

        protected override void OnConnect(int cell1, int cell2)
        {
            SingleChannelController channelController = VirtualCircuitKey as SingleChannelController;
            if (channelController == null || channelController.IsInvalid() )
            {
                return;
            }
            var manager = GetNetworkManager();
            if (manager is UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem)
                electricalConduitSystem.AddSemiVirtualLink(cell1, VirtualCircuitKey);
        }
        
    }
}