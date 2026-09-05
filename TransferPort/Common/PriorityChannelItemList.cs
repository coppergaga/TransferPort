using System.Collections.Generic;
using System.Linq;

namespace RsTransferPort {
    public class PriorityChannelItemList {
        // 这里的下标代表了可选的优先级, 游戏中是1-9, 对应这里的0-8
        private readonly List<PortItem>[] priorityList = new List<PortItem>[9];

        public PriorityChannelItemList() {
            for (int i = 0; i < 9; i++) {
                priorityList[i] = new List<PortItem>();
            }
        }

        public void AddChannelItem(PortItem item) {
            priorityList[item.Priority - 1].Add(item);
        }

        public void ItemPriorityChange(PortItem item, int newPriority, int oldPriority) {
            RemoveChannelItem(item);
            AddChannelItem(item);
        }

        public void RemoveChannelItem(PortItem item) {
            for (int i = priorityList.Length - 1; i >= 0; i--) {
                if (priorityList[i].Remove(item)) {
                    return;
                }
            }
        }

        public int GetItemCountByPriority(int priority) {
            return priorityList[priority - 1].Count;
        }

        public int[] AllPriority() {
            HashSet<int> ret = new HashSet<int>();
            for (int i = 0; i < priorityList.Length; i++) {
                if (priorityList[i].Count > 0) { ret.Add(i + 1); }
            }
            return ret.ToArray();
        }

        private readonly List<PortItem> _items = new List<PortItem>();
        public List<PortItem> Items {
            get {
                _items.Clear();
                for (int i = priorityList.Length - 1; i >= 0; i--) {
                    _items.AddRange(priorityList[i]);
                }
                return _items;
            }
        }

        private bool TryGetLocalIndex(int flatPos, out int bucketIdx, out int localIdx) {
            bucketIdx = -1; localIdx = -1;
            if (flatPos < 0) { return false; }
            int sum = 0;
            for (int i = priorityList.Length - 1; i >= 0; i--) {
                var cnt = priorityList[i].Count;
                if (flatPos < sum + cnt) {
                    bucketIdx = i;
                    localIdx = flatPos - sum;
                    return true;
                }
                sum += cnt;
            }
            return false;
        }

        private void RotateListInPlace(int bucketIdx, int splitIdx) {
            var list = priorityList[bucketIdx];
            int cnt = list.Count;
            if (splitIdx <= 0 || splitIdx >= cnt) { return; }
            list.Reverse(0, splitIdx);
            list.Reverse(splitIdx, cnt - splitIdx);
            list.Reverse(0, cnt);
        }

        public void RotateByFlatPosition(int flatPos) {
            if (TryGetLocalIndex(flatPos, out int bucketIdx, out int localIdx)) {
                RotateListInPlace(bucketIdx, localIdx + 1);
            }
        }
    }
}