
namespace RsTransferPort {
    public class WirelessLogicPortChannel : SingleChannelController {
        protected override void OnAfterAdd(PortItem item) {
            base.OnAfterAdd(item);
            if (IsInvalid()) { return; }

            SyncSignal();
        }

        protected override void OnPreRemove(PortItem item) {
            base.OnPreRemove(item);
            if (IsInvalid()) { return; }

            if (item.InOutTypo == InOutType.Receiver &&
                item.GG_TryGetCmpFast<WirelessLogicPort>(out var rwlp)) {
                rwlp.SendSignal(0);
            }
        }

        protected override void OnAfterRemove() {
            base.OnAfterRemove();
            SyncSignal();
        }

        /// <summary>
        ///     输入逻辑信号改变
        /// </summary>
        public void SyncSignal() {
            //开始同步信号
            if (receivers.Count == 0) return;
            int signal = 0;
            foreach (PortItem sender in senders) {
                if (sender.GG_TryGetCmpFast<WirelessLogicPort>(out var swlp)) {
                    signal |= swlp.GetInputSignal();
                }
                if (signal > 0) { break; }
            }
            foreach (PortItem receiver in receivers) {
                if (receiver.GG_TryGetCmpFast<WirelessLogicPort>(out var rwlp)) {
                    rwlp.SendSignal(signal);
                }
            }
        }

        public WirelessLogicPortChannel(BuildingType buildingType, string channelName, int worldIdAG) : base(buildingType, channelName, worldIdAG) {
        }
    }
}