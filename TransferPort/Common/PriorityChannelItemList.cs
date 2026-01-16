using System.Collections;
using System.Collections.Generic;
using RsLib.Collections;

namespace RsTransferPort {
    public class PriorityChannelItemList : IList<PriorityChannelItemInfo> {
        private RsSortedList<PriorityChannelItemInfo> priorityList = new RsSortedList<PriorityChannelItemInfo>();

        public void Add(PriorityChannelItemInfo item) {
            throw new System.NotImplementedException();
        }

        public void Clear() {
            priorityList.Clear();
        }

        public bool Contains(PriorityChannelItemInfo item) {
            throw new System.NotImplementedException();
        }

        public void CopyTo(PriorityChannelItemInfo[] array, int arrayIndex) {
            throw new System.NotImplementedException();
        }

        public bool Remove(PriorityChannelItemInfo item) {
            throw new System.NotImplementedException();
        }

        public int Count => priorityList.Count;
        public bool IsReadOnly => true;

        public void AddChannelItem(PortItem item) {
            AddChannelItem(item, true);
        }

        private void AddChannelItem(PortItem item, bool addEvent) {
            PriorityChannelItemInfo itemInfo = GetOrAddPriorityInfo(item.Priority);
            itemInfo.items.Add(item);
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
                if (info.items.Remove(item)) {
                    if (removeEvent) {
                        item.OnPriorityChange -= ItemOnOnPriorityChange;
                    }
                    if (info.items.Count == 0) {
                        //移除
                        priorityList.Remove(info);
                    }
                    return;
                }
            }
        }

        private PriorityChannelItemInfo GetOrAddPriorityInfo(int priority) {
            foreach (PriorityChannelItemInfo itemInfo in priorityList) {
                if (itemInfo.priority == priority) {
                    return itemInfo;
                }
            }

            PriorityChannelItemInfo info = new PriorityChannelItemInfo();
            info.priority = priority;
            priorityList.Add(info);
            return info;
        }

        public PriorityChannelItemInfo GetByPriority(int priority) {
            foreach (PriorityChannelItemInfo itemInfo in priorityList) {
                if (itemInfo.priority == priority) {
                    return itemInfo;
                }
            }

            return null;
        }


        public IEnumerator<PriorityChannelItemInfo> GetEnumerator() {
            return priorityList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int IndexOf(PriorityChannelItemInfo item) {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, PriorityChannelItemInfo item) {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index) {
            throw new System.NotImplementedException();
        }

        public PriorityChannelItemInfo this[int index] {
            get => priorityList[index];
            set => throw new System.NotImplementedException();
        }

        public int[] AllPriority() {
            int[] priorities = new int[priorityList.Count];
            for (var i = 0; i < priorityList.Count; i++) {
                priorities[i] = priorityList[i].priority;
            }

            return priorities;
        }
    }
}