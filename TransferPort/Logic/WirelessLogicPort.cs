
namespace RsTransferPort {
    public class WirelessLogicPort : KMonoBehaviour {
        [MyCmpGet] private LogicPorts logicPorts;
        [MyCmpGet] private PortItem item;

        private static readonly EventSystem.IntraObjectHandler<WirelessLogicPort> OnLogicValueChangeDelegate =
            new EventSystem.IntraObjectHandler<WirelessLogicPort>(delegate (WirelessLogicPort cmp, object data) { cmp.OnLogicValueChanged(data); });

        protected override void OnSpawn() {
            base.OnSpawn();
            if (item.InOutTypo == InOutType.Sender) {
                Subscribe((int)GameHashes.LogicEvent, OnLogicValueChangeDelegate);
            }
        }

        protected override void OnCleanUp() {
            if (item.InOutTypo == InOutType.Sender) {
                Unsubscribe((int)GameHashes.LogicEvent, OnLogicValueChangeDelegate);
            }
            base.OnCleanUp();
        }

        public int GetInputSignal() {
            return logicPorts.GetInputValue(WirelessLogicConfig.WirelessLogicPortID);
        }

        public void SendSignal(int signal) {
            logicPorts.SendSignal(WirelessLogicConfig.WirelessLogicPortID, signal);
        }

        private void OnLogicValueChanged(object data) {
            var controller = PortManager.Instance.GetChannelController(item);
            if (!(controller is WirelessLogicPortChannel cc)) { return; }
            cc.SyncSignal();
        }
    }
}