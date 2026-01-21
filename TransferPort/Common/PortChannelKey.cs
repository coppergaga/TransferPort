using System;
using RsLib;

namespace RsTransferPort {
    public readonly struct PortChannelKey : IEquatable<PortChannelKey> {
        public static readonly PortChannelKey Invalid = default;

        private readonly string name;
        private readonly int worldId;
        private readonly BuildingType buildingType;

        public PortChannelKey(string name, int worldId, BuildingType buildingType) {
            this.name = name ?? string.Empty;
            this.worldId = worldId;
            this.buildingType = buildingType;
        }

        public bool Equals(PortChannelKey other) {
            return IsSame(other.buildingType) && IsSame(other.worldId) && IsSame(other.name);
        }

        public override bool Equals(object obj) {
            return obj is PortChannelKey other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = name.GetHashCode();
                hashCode = (hashCode * 397) ^ worldId;
                hashCode = (hashCode * 397) ^ (int)buildingType;
                return hashCode;
            }
        }

        public override string ToString() {
            return RsObjectDebug.BuildAllField(this);
        }

        public static bool operator ==(PortChannelKey a, PortChannelKey b) => Equals(a, b);

        public static bool operator !=(PortChannelKey a, PortChannelKey b) => !Equals(a, b);

        public bool IsSame(BuildingType bt) => buildingType == bt;
        public bool IsSame(int worldID) => worldId == worldID;
        public bool IsSame(string channelName) => string.Equals(name, channelName, StringComparison.Ordinal);
    }
}