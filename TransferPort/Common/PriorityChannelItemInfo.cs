using System;
using System.Collections.Generic;

namespace RsTransferPort {
    public class PriorityChannelItemInfo : IComparable<PriorityChannelItemInfo> {
        public int priority = 0;
        public List<TransferPortChannel> items = new List<TransferPortChannel>();
        /// <summary>
        /// 轮询索引
        /// </summary>
        public int pollIndex;

        public void PollIndexUp() {
            pollIndex++;
        }

        /// <summary>
        /// 根据items自增+1然后循环
        /// </summary>
        public void PollIndexUpAndRedress() {
            pollIndex++;
            PollIndexRedress();
        }

        /// <summary>
        /// 纠正
        /// </summary>
        public void PollIndexRedress() {
            if (pollIndex < 0 || pollIndex >= items.Count) {
                pollIndex = 0;
            }
        }

        public TransferPortChannel GetItemByPollIndex() {
            return items[pollIndex];
        }

        public int CompareTo(PriorityChannelItemInfo other) {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            if (priority > other.priority)
                return -1;
            return priority < other.priority ? 1 : 0;
        }
    }
}