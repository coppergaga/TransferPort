
namespace RsTransferPort {
    public class TransferConduitChannel : SingleChannelController {
        public readonly PriorityChannelItemList senderPriorityList = new PriorityChannelItemList();
        public readonly PriorityChannelItemList recverPriorityList = new PriorityChannelItemList();
        protected override void OnAfterAdd(PortItem item) {
            base.OnAfterAdd(item);
            GetPriorityList(item).AddChannelItem(item);
        }

        protected override void OnPreRemove(PortItem item) {
            base.OnPreRemove(item);
            GetPriorityList(item).RemoveChannelItem(item);
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
            if (IsInvalid()) { return; }
            if (senders.Count == 0 || receivers.Count == 0) { return; }
            SimConduitUpdate();
        }

        private int tttt = 0;
        private void SimConduitUpdate() {
            // 这里做高优先级优先匹配的逻辑
            var senderItems = senderPriorityList.Items;
            var recverItems = recverPriorityList.Items;
            int i = 0, j = 0;
            for (; i < senderItems.Count; i++) {
                if (!senderItems[i].GG_TryGetCmpFast<TransferConduit>(out var sendertc)) {
                    continue;
                }
                for (; j < recverItems.Count; j++) {
                    if (IsConduitEmpty(sendertc.ConduitIOCell)) { break; }      // 只要这个发送端还有内容物就持续找出口, 以此实现一传多
                    if (!recverItems[j].GG_TryGetCmpFast<TransferConduit>(out var receivertc)) {
                        continue;
                    }
                    if (!ConduitTransfer(sendertc.ConduitIOCell, receivertc.ConduitIOCell)) { break; }  // 这里实现多传一
                }
                if (j >= recverItems.Count) { break; }
            }
            // 传送完成后整理内部排列, 模拟游戏中一根管道分多叉后轮流输出的逻辑
            senderPriorityList.RotateByFlatPosition(i - 1);
            recverPriorityList.RotateByFlatPosition(j - 1);
        }

        /// 只要 液体气体接收端 没满 就不找下一个出口, 以此实现多传一
        /// 轨道内部逻辑不允许合并, 所以不做这个逻辑
        /// <returns>是否寻找下一个出口</returns>
        private bool ConduitTransfer(int inputCell, int outputCell) {
            if (BuildingTypo == BuildingType.Solid) {
                SolidConduitFlow flow = (SolidConduitFlow)GetConduitManager();
                if (flow.HasConduit(outputCell) && !flow.IsConduitFull(outputCell)) {
                    var pickupable = flow.RemovePickupable(inputCell);
                    if (pickupable) { flow.AddPickupable(outputCell, pickupable); }
                }
                return true;
            }
            else {
                ConduitFlow flow = (ConduitFlow)GetConduitManager();
                if (flow.HasConduit(outputCell) && !flow.IsConduitFull(outputCell)) {
                    var ic = flow.GetContents(inputCell);
                    var useMass = flow.AddElement(outputCell, ic.element, ic.mass, ic.temperature, ic.diseaseIdx, ic.diseaseCount);
                    if (useMass > 0) {
                        flow.RemoveElement(inputCell, useMass);
                        if (!flow.IsConduitFull(outputCell)) { return false; } // 这里是相同元素合并后管道没满, 应该只有这种情况不找下一个出口, 而是找下一个入口来实现多传一
                    } // 这里 useMass <= 0 意味着元素是不同的, 因为先判断了管道是否已满, 所以需要找下一个出口
                }     // 这里直接就是管道满了或者根本没有管道, 也找下一个出口
                return true;
            }
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

        public void ItemPriorityChange(PortItem item, int newPriority, int oldPriority) {
            GetPriorityList(item).ItemPriorityChange(item, newPriority, oldPriority);
        }

        public TransferConduitChannel(BuildingType buildingType, string channelName, int worldIdAG) : base(buildingType, channelName, worldIdAG) {
        }

        private PriorityChannelItemList GetPriorityList(PortItem item) => item.InOutTypo == InOutType.Sender ? senderPriorityList : recverPriorityList;
    }
}