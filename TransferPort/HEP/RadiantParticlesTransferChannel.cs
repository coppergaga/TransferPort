namespace RsTransferPort {
    public class RadiantParticlesTransferChannel : SingleChannelController {
        public RadiantParticlesTransferChannel(BuildingType buildingType, string channelName, int worldIdAG) : base(buildingType, channelName, worldIdAG) {
        }

        private int senderIndex;
        private int receiverIndex;

        protected override void OnAfterAdd(PortItem item) {
            base.OnAfterAdd(item);
            if (IsInvalid()) return;
            if (item.InOutTypo == InOutType.Sender) {
                item.Subscribe((int)GameHashes.OnParticleStorageChanged, OnParticleStorageChanged);
            }
            else {
                item.Subscribe((int)GameHashes.OperationalChanged, OnReceiverOperationalChange);
            }
            SyncSignal();
        }

        protected override void OnPreRemove(PortItem item) {
            base.OnPreRemove(item);
            if (IsInvalid()) return;
            if (item.InOutTypo == InOutType.Sender) {
                item.Unsubscribe((int)GameHashes.OnParticleStorageChanged, OnParticleStorageChanged);
                if (item.GG_TryGetCmpFast<RadiantParticlesTransferSender>(out var rpts)) {
                    rpts.ConfigReceiverAllow(false);
                }
            }
            else {
                item.Unsubscribe((int)GameHashes.OperationalChanged, OnReceiverOperationalChange);
            }
        }

        protected override void OnAfterRemove() {
            base.OnAfterRemove();
            SyncSignal();
        }

        private void OnParticleStorageChanged(object data) {
            Update();
        }

        private void Update() {
            var receiverIndexCount = 0; //循环次数计算
            for (var i = 0; i < senders.Count; i++) {
                if (receiverIndexCount == receivers.Count) return;

                senderIndex %= senders.Count;
                if (!senders[senderIndex].GG_TryGetCmpFast<RadiantParticlesTransferSender>(out var rpts) ||
                    !rpts.HasRadiation()) {
                    senderIndex++;
                    continue;
                }
                if (receiverIndex >= receivers.Count) { receiverIndex = 0; }
                while (receiverIndexCount < receivers.Count) {
                    var receiverItem = receivers[receiverIndex];
                    receiverIndex = ++receiverIndex % receivers.Count;
                    receiverIndexCount++;
                    if (receiverItem.GG_TryGetCmpFast<RadiantParticlesTransferReceiver>(out var rptr) &&
                        rptr.Transmissible()) {
                        //这里需要计算入口到出口的距离，销毁一定量的粒子
                        rptr.StoreAndLaunch(rpts.ConsumeAll());
                        senderIndex++;
                        break;
                    }
                }
            }
        }

        public void OnReceiverOperationalChange(object data) {
            SyncSignal();
        }

        /// <summary>
        ///     输入逻辑信号改变
        /// </summary>
        public void SyncSignal() {
            var signal = HasOutletEnable();
            foreach (PortItem sender in senders) {
                if (sender.GG_TryGetCmpFast<RadiantParticlesTransferSender>(out var rpts)) {
                    rpts.ConfigReceiverAllow(signal);
                }
            }
        }

        private bool HasOutletEnable() {
            foreach (PortItem item in receivers) {
                if (item.GG_TryGetCmpFast<RadiantParticlesTransferReceiver>(out var rptr) &&
                    rptr.Transmissible()) {
                    return true;
                }
            }
            return false;
        }
    }
}