namespace RsTransferPort {
    public class RadiantParticlesTransferChannel : SingleChannelController {
        public RadiantParticlesTransferChannel(BuildingType buildingType, string channelName, int worldIdAG) : base(
            buildingType, channelName, worldIdAG) {
        }

        private int senderIndex;
        private int receiverIndex;

        protected override void OnAdd(PortItem item) {
            if (IsInvalid()) return;
            if (item.InOutType == InOutType.Sender) {
                item.Subscribe((int)GameHashes.OnParticleStorageChanged, OnParticleStorageChanged);
            }
            else {
                item.Subscribe((int)GameHashes.OperationalChanged, OnReceiverOperationalChange);
            }

            SyncSignal();
        }

        protected override void OnRemove(PortItem item) {
            if (IsInvalid()) return;
            if (item.InOutType == InOutType.Sender) {
                item.Unsubscribe((int)GameHashes.OnParticleStorageChanged, OnParticleStorageChanged);
                item.GetComponent<RadiantParticlesTransferSender>().ReceiverAllow = false;
            }
            else {
                item.Unsubscribe((int)GameHashes.OperationalChanged, OnReceiverOperationalChange);
            }

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
                var sender = senders[senderIndex].GetComponent<RadiantParticlesTransferSender>();
                if (!sender.storage.HasRadiation()) {
                    senderIndex++;
                    continue;
                }

                if (receiverIndex >= receivers.Count) { receiverIndex = 0; }
                while (receiverIndexCount < receivers.Count) {
                    var receiver = receivers[receiverIndex].GetComponent<RadiantParticlesTransferReceiver>();
                    receiverIndex = ++receiverIndex % receivers.Count;
                    receiverIndexCount++;
                    if (receiver.Transmissible) {
                        float amount = sender.storage.ConsumeAll();
                        //这里需要计算入口到出口的距离，销毁一定量的粒子
                        receiver.StoreAndLaunch(amount);
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
                RadiantParticlesTransferSender particlesTransferSender =
                    sender.GetComponent<RadiantParticlesTransferSender>();
                if (particlesTransferSender != null) {
                    particlesTransferSender.ReceiverAllow = signal;
                }
            }
        }

        private bool HasOutletEnable() {
            foreach (PortItem receiver in receivers) {
                RadiantParticlesTransferReceiver transferReceiver = receiver.GetComponent<RadiantParticlesTransferReceiver>();
                if (transferReceiver != null && transferReceiver.Transmissible) {
                    return true;
                }
            }

            return false;
        }
    }
}