
namespace RsTransferPort {
    public class WirelessLogicPort : KMonoBehaviour {
        [MyCmpGet] private LogicPorts logicPorts;
        [MyCmpGet] private PortItem item;

        protected override void OnSpawn() {
            base.OnSpawn();
            item.HandleReturnInt = GetInputSignal;
            item.HandleInParamInt = SendSignal;
        }

        protected override void OnCleanUp() {
            if (!Util.IsNullOrDestroyed(item)) {
                item.HandleReturnInt = null;
                item.HandleInParamInt = null;
            }
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