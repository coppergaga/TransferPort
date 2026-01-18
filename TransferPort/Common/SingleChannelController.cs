using System;
using System.Collections.Generic;

namespace RsTransferPort {
    public class SingleChannelController : IComparable<SingleChannelController> {
        public BuildingType BuildingType { get; }
        public string ChannelName { get; }

        public List<PortItem> senders = new List<PortItem>();
        public List<PortItem> receivers = new List<PortItem>();
        public List<PortItem> all = new List<PortItem>();

        public int Total => all.Count;
        public int WorldIdAG { get; } // -1表示全球 

        public bool IsGlobal => WorldIdAG == -1;

        public string DisplayChannelName {
            get {
                if (IsInvalid()) {
                    return STRINGS.UI.SIDESCREEN.RS_PORT_CHANNEL.CHANNEL_NULL.ToString();
                }

                return ChannelName;
            }
        }

        public SingleChannelController(BuildingType buildingType, string channelName, int worldIdAG) {
            BuildingType = buildingType;
            ChannelName = channelName;
            WorldIdAG = worldIdAG;
            OnInit();
        }

        public void Add(PortItem item) {

            if (item == null) {
                Debug.LogErrorFormat("SingleChannelController buildingType:{0} Add null", BuildingType);
                return;
            }

            if (item.InOutType == InOutType.Receiver)
                receivers.Add(item);
            else
                senders.Add(item);
            all.Add(item);
            OnAdd(item);
        }

        public void Remove(PortItem item) {
            if (item == null) {
                return;
            }
            if (item.InOutType == InOutType.Receiver)
                receivers.Remove(item);
            else
                senders.Remove(item);
            all.Remove(item);
            OnRemove(item);
        }

        protected virtual void OnAdd(PortItem item) {
        }

        protected virtual void OnRemove(PortItem item) {

        }

        public virtual bool Contains(PortItem item) {
            return all.Contains(item);
        }

        protected virtual void OnInit() {

        }
        public virtual void OnSpawn() {
        }

        public virtual void OnConUpdate() {
        }

        public virtual void OnCleanUp() {
            senders = null;
            receivers = null;
            all = null;
        }

        public bool IsInvalid() {
            return string.IsNullOrEmpty(ChannelName);
        }


        public int CompareTo(SingleChannelController other) {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return string.Compare(ChannelName, other.ChannelName, StringComparison.Ordinal);
        }

        public ICollection<WorldContainer> GetIncludeWorldContainer() {
            HashSet<WorldContainer> worldContainers = new HashSet<WorldContainer>();
            foreach (PortItem channel in all) {
                WorldContainer myWorld = channel.GetMyWorld();
                if (myWorld != null) {
                    worldContainers.Add(myWorld);
                }
            }
            return worldContainers;
        }
    }
}