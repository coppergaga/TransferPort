using System;
using RsLib;

namespace RsTransferPort {
    public struct PortChannelKey : IEquatable<PortChannelKey> {
        public static PortChannelKey Invalid = default;

        public readonly string name;
        public readonly int worldId;
        public readonly BuildingType buildingType;

        public PortChannelKey(string name, int worldId, BuildingType buildingType) {
            this.name = name;
            this.worldId = worldId;
            this.buildingType = buildingType;
        }

        public bool Equals(PortChannelKey other) {
            return name == other.name && worldId == other.worldId && buildingType == other.buildingType;
        }

        public override bool Equals(object obj) {
            return obj is PortChannelKey other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = name != null ? name.GetHashCode() : 0;
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
    }
}