using System;
using System.Collections.Generic;

namespace RsTransferPort {
    public class PriorityChannelItemInfo : IComparable<PriorityChannelItemInfo> {
        private readonly List<PortItem> _items = new List<PortItem>();

        public int Priority { get; set; } = 0;
        public int Count => _items.Count;
        public List<PortItem> AllItems => _items;

        public void Add(PortItem item) { _items.Add(item); }
        public bool Remove(PortItem item) { return _items.Remove(item); }

        public int CompareTo(PriorityChannelItemInfo other) {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            if (Priority > other.Priority)
                return -1;
            return Priority < other.Priority ? 1 : 0;
        }
    }
}