
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
            switch (BuildingTypo) {
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
            SimConduitUpdate();
        }


        private void SimConduitUpdate() {
            var senderItems = senderPriorityList.Items;
            var receiverItems = receiverPriorityList.Items;
            for (int i = 0, j = 0; i < senderItems.Count; i++) {
                int inputCell = senderItems[i].HandleReturnInt();
                if (IsConduitEmpty(inputCell)) { continue; }
                while (j < receiverItems.Count) {
                    int outputCell = receiverItems[j].HandleReturnInt();
                    j++;
                    if (ConduitTransfer(inputCell, outputCell)) { break; }
                }
            }
        }

        /// <returns>是否传送了</returns>
        private bool ConduitTransfer(int inputCell, int outputCell) {
            if (BuildingTypo == BuildingType.Solid) {
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
                    var ic = flow.GetContents(inputCell);
                    var useMass = flow.AddElement(outputCell, ic.element, ic.mass, ic.temperature, ic.diseaseIdx, ic.diseaseCount);
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
            if (BuildingTypo == BuildingType.Solid) {
                SolidConduitFlow flow = (SolidConduitFlow)GetConduitManager();
                return !flow.HasConduit(cell) || flow.IsConduitEmpty(cell);
            }
            else {
                ConduitFlow flow = (ConduitFlow)GetConduitManager();
                return !flow.HasConduit(cell) || flow.IsConduitEmpty(cell);
                    
            }
        }

        public TransferConduitChannel(BuildingType buildingType, string channelName, int worldIdAG) : base(buildingType, channelName, worldIdAG) {
        }
    }
}