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
            UIScheduler.Instance.Schedule("RadiantParticlesTransferChannelOnAdd", 0f, (_) => { if (!Util.IsNullOrDestroyed(this)) { this.SyncSignal(); } });
            //SyncSignal();
        }

        protected override void OnRemove(PortItem item) {
            if (IsInvalid()) return;
            if (item.InOutType == InOutType.Sender) {
                item.Unsubscribe((int)GameHashes.OnParticleStorageChanged, OnParticleStorageChanged);
                item.HandleInParamInt(RsLib.RsUtil.IntFrom(false));
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
                var senderItem = senders[senderIndex];
                if (!RsLib.RsUtil.BoolFrom(senderItem.HandleReturnInt())) {
                    senderIndex++;
                    continue;
                }

                if (receiverIndex >= receivers.Count) { receiverIndex = 0; }
                while (receiverIndexCount < receivers.Count) {
                    var receiverItem = receivers[receiverIndex];
                    receiverIndex = ++receiverIndex % receivers.Count;
                    receiverIndexCount++;
                    if (RsLib.RsUtil.BoolFrom(receiverItem.HandleReturnInt())) {
                        float amount = senderItem.HandleReturnFloat();
                        //这里需要计算入口到出口的距离，销毁一定量的粒子
                        receiverItem.HandleInParamFloat(amount);
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
                sender.HandleInParamInt(RsLib.RsUtil.IntFrom(signal));
            }
        }

        private bool HasOutletEnable() {
            foreach (PortItem item in receivers) {
                if (RsLib.RsUtil.BoolFrom(item.HandleReturnInt())) { return true; }
            }
            return false;
        }
    }
}