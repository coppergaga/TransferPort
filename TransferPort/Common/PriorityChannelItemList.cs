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
            for (var i = 0; i < priorityList.Length; i++) {
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
    }
}