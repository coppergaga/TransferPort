using System.Collections.Generic;
using RsLib.Collections;

namespace RsTransferPort {
    public class PriorityChannelItemList {
        private RsSortedList<PriorityChannelItemInfo> priorityList = new RsSortedList<PriorityChannelItemInfo>();

        public void AddChannelItem(PortItem item) {
            AddChannelItem(item, true);
        }

        private void AddChannelItem(PortItem item, bool addEvent) {
            PriorityChannelItemInfo itemInfo = GetOrAddPriorityInfo(item.Priority);
            itemInfo.Add(item);
            if (addEvent) {
                item.OnPriorityChange += ItemOnOnPriorityChange;
            }
        }

        private void ItemOnOnPriorityChange(PortItem channel, int newPriority, int oldPriority) {
            RemoveChannelItem(channel, false);
            AddChannelItem(channel, false);
        }

        public void RemoveChannelItem(PortItem item) {
            RemoveChannelItem(item, true);
        }

        public void RemoveChannelItem(PortItem item, bool removeEvent) {
            foreach (PriorityChannelItemInfo info in priorityList) {
                if (info.Remove(item)) {
                    if (removeEvent) {
                        item.OnPriorityChange -= ItemOnOnPriorityChange;
                    }
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