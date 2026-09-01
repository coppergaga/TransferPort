using System.Collections.Generic;
using RsLib.Collections;

namespace RsTransferPort {
    public class PriorityChannelItemList {
        private RsSortedList<PriorityChannelItemInfo> priorityList = new RsSortedList<PriorityChannelItemInfo>();

        public void AddChannelItem(PortItem item) {
            PriorityChannelItemInfo itemInfo = GetOrAddPriorityInfo(item.Priority);
            itemInfo.Add(item);
        }

        public void ItemPriorityChange(PortItem item, int newPriority, int oldPriority) {
            RemoveChannelItem(item);
            AddChannelItem(item);
        }

        public void RemoveChannelItem(PortItem item) {
            for (int i = priorityList.Count - 1; i >= 0; i--) {
                PriorityChannelItemInfo info = priorityList[i];
                if (info.Remove(item)) {
                    if (info.Count == 0) {
                        priorityList.Remove(info);
                    }
                    return;
                }
            }
        }

        private PriorityChannelItemInfo GetOrAddPriorityInfo(int priority) {
            foreach (PriorityChannelItemInfo itemInfo in priorityList) {
                if (itemInfo.Priority == priority) {
                    return itemInfo;
                }
            }

            PriorityChannelItemInfo info = new PriorityChannelItemInfo {
                Priority = priority
            };
            priorityList.Add(info);
            return info;
        }

        public PriorityChannelItemInfo GetByPriority(int priority) {
            foreach (PriorityChannelItemInfo itemInfo in priorityList) {
                if (itemInfo.Priority == priority) {
                    return itemInfo;
                }
            }

            return null;
        }

        public int[] AllPriority() {
            int[] priorities = new int[priorityList.Count];
            for (var i = 0; i < priorityList.Count; i++) {
                priorities[i] = priorityList[i].Priority;
            }

            return priorities;
        }

        private readonly List<PortItem> _items = new List<PortItem>();
        public List<PortItem> Items {
            get {
                _items.Clear();
                for (int i = 0; i < priorityList.Count; i++) {
                    _items.AddRange(priorityList[i].AllItems);
                }
                return _items;
            }
        }
    }
}