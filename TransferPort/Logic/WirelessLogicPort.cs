
namespace RsTransferPort {
    public class WirelessLogicPort : KMonoBehaviour {
        [MyCmpReq] private LogicPorts logicPorts;
        [MyCmpReq] private PortItem item;

        protected override void OnSpawn() {
            base.OnSpawn();
            item.HandleReturnInt = GetInputSignal;
            item.HandleInParamInt = SendSignal;
        }

        protected override void OnCleanUp() {
            item.HandleReturnInt = null;
            item.HandleInParamInt = null;
            base.OnCleanUp();
        }

        private int GetInputSignal() {
            return logicPorts.GetInputValue(WirelessLogicConfig.WirelessLogicPortID);
        }

        private void SendSignal(int signal) {
            logicPorts.SendSignal(WirelessLogicConfig.WirelessLogicPortID, signal);
        }
    }
}