
namespace RsTransferPort {
    public class TransferConduitChannel : SingleChannelController {
        public readonly PriorityChannelItemList senderPriorityList = new PriorityChannelItemList();
        public readonly PriorityChannelItemList receiverPriorityList = new PriorityChannelItemList();
        protected override void OnAfterAdd(PortItem item) {
            base.OnAfterAdd(item);
            if (item.InOutType == InOutType.Sender) {
                senderPriorityList.AddChannelItem(item);
            }
            else {
                receiverPriorityList.AddChannelItem(item);
            }
        }

        protected override void OnPreRemove(PortItem item) {
            base.OnPreRemove(item);
            if (item.InOutType == InOutType.Sender) {
                senderPriorityList.RemoveChannelItem(item);
            }
            else {
                receiverPriorityList.RemoveChannelItem(item);
            }
        }

        public IConduitFlow GetConduitManager() {
            switch (BuildingType) {
                case BuildingType.Gas:
                    return Game.Instance.gasConduitFlow;
                case BuildingType.Liquid:
                    return Game.Instance.liquidConduitFlow;
                case BuildingType.Solid:
                    return Game.Instance.solidConduitFlow;
                default:
                    return null;
            }
        }

        public void ConduitUpdate(float dt) {
            if (IsInvalid()) {
                return;
            }

            if (senders.Count == 0 || receivers.Count == 0) return;
            ConduitUpdate1();
        }


        private void ConduitUpdate1() {
            int cpReceiverEachCount = 0; //循环次数计算
            int rpIndex = 0; //接收端的优先级信息的索引
            //int senderIndex = 0;
            //设置一次只能传送一种液体
            for (int senderIndex = 0; senderIndex < senderPriorityList.Count; senderIndex++) {
                var prioritySenderInfo = senderPriorityList[senderIndex];
                //纠正值
                // prioritySenderInfo.PollIndexRedress();
                for (int i = 0; i < prioritySenderInfo.items.Count; i++) {
                    //获取当前权重值的循环索引
                    prioritySenderInfo.PollIndexRedress();
                    int inputCell = prioritySenderInfo.GetItemByPollIndex().HandleReturnInt();
                    if (IsConduitEmpty(inputCell)) {
                        prioritySenderInfo.PollIndexUp();
                        continue;
                    }
                    if (!ConduitUpdate2(inputCell, ref cpReceiverEachCount, senderIndex, ref rpIndex)) {
                        //找不到就退出去
                        return;
                    }
                }
            }
        }

        /// <returns>有出口接收传送了</returns>
        private bool ConduitUpdate2(int inputCell, ref int cpReceiverEachCount, int senderIndex, ref int rpIndex) {
            for (; rpIndex < receiverPriorityList.Count; rpIndex++) {
                PriorityChannelItemInfo priorityReceiverInfo = receiverPriorityList[rpIndex];
                while (cpReceiverEachCount <= priorityReceiverInfo.items.Count) {
                    cpReceiverEachCount++;
                    priorityReceiverInfo.PollIndexRedress();
                    int outputCell = priorityReceiverInfo.GetItemByPollIndex().HandleReturnInt();
                    priorityReceiverInfo.PollIndexUp();
                    if (ConduitTransfer(inputCell, outputCell)) {
                        senderPriorityList[senderIndex].PollIndexUp();
                        return true;
                    }
                }

                cpReceiverEachCount = 0;
            }
            return false;
        }

        /// <returns>是否传送了</returns>
        private bool ConduitTransfer(int inputCell, int outputCell) {
            if (BuildingType == BuildingType.Solid) {
                SolidConduitFlow flow = (SolidConduitFlow)GetConduitManager();
                if (flow.HasConduit(outputCell) && flow.IsConduitEmpty(outputCell)) {
                    var pickupable = flow.RemovePickupable(inputCell);
                    if (pickupable) flow.AddPickupable(outputCell, pickupable);
                    return true; //直接返回
                }
            }
            else {
                ConduitFlow flow = (ConduitFlow)GetConduitManager();
                if (flow.HasConduit(outputCell) && flow.IsConduitEmpty(outputCell)) {
                    var inputContents = flow.GetContents(inputCell);
                    var useMass = flow.AddElement(outputCell, inputContents.element, inputContents.mass,
                        inputContents.temperature,
                        inputContents.diseaseIdx, inputContents.diseaseCount);
                    flow.RemoveElement(inputCell, useMass);
                    return true; //直接返回
                }
            }

            return false;
        }

        /// <summary>
        /// 输入端管道判断
        /// </summary>
        private bool IsConduitEmpty(int cell) {
            if (BuildingType == BuildingType.Solid) {
                SolidConduitFlow flow = (SolidConduitFlow)GetConduitManager();
                return !flow.HasConduit(cell) || flow.IsConduitEmpty(cell);
            }
            else {
                ConduitFlow flow = (ConduitFlow)GetConduitManager();
                return !flow.HasConduit(cell) || flow.IsConduitEmpty(cell);
            }
        }

        public void OnCleanup() {
        }


        public TransferConduitChannel(BuildingType buildingType, string channelName, int worldIdAG) : base(buildingType, channelName, worldIdAG) {
        }
    }
}