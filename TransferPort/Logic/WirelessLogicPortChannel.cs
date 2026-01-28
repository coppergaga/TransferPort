
namespace RsTransferPort {
    public class WirelessLogicPortChannel : SingleChannelController {
        protected override void OnAfterAdd(PortItem item) {
            base.OnAfterAdd(item);
            if (IsInvalid()) { return; }

            if (item.InOutType == InOutType.Sender) {
                item.Subscribe((int)GameHashes.LogicEvent, OnInputLogicEvent);
            }
            SyncSignal();
        }

        protected override void OnPreRemove(PortItem item) {
            base.OnPreRemove(item);
            if (IsInvalid()) { return; }

            if (item.InOutType == InOutType.Sender) {
                item.Unsubscribe((int)GameHashes.LogicEvent, OnInputLogicEvent);
            }

            if (item.InOutType == InOutType.Receiver) {
                item.HandleInParamInt?.Invoke(0);
            }
        }

        protected override void OnAfterRemove() {
            base.OnAfterRemove();
            SyncSignal();
        }

        public void OnInputLogicEvent(object data) {
            SyncSignal();
        }

        /// <summary>
        ///     输入逻辑信号改变
        /// </summary>
        public void SyncSignal() {
            //开始同步信号
            if (receivers.Count == 0) return;
            var signal = GetSignal();
            foreach (PortItem receiver in receivers) { receiver.HandleInParamInt(signal); }
        }

        /// <summary>
        ///     通过计算所有输入端的信号状态，返回当前通道的信号状态
        /// </summary>
        public int GetSignal() {
            int signal = 0;
            foreach (PortItem sender in senders) { signal |= sender.HandleReturnInt(); }
            return signal;
        }

        public WirelessLogicPortChannel(BuildingType buildingType, string channelName, int worldIdAG) : base(buildingType, channelName, worldIdAG) {
        }
    }
}