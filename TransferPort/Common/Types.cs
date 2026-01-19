namespace RsTransferPort {
    public enum InOutType {
        None,
        Sender,
        Receiver
    }

    public enum BuildingType {
        None,
        Gas,
        Liquid,
        Solid,
        Power,
        Logic,
        HEP,
    }

    public class Converter {
        public static BuildingType FromConduitType(ConduitType conduitType) {
            switch (conduitType) {
                case ConduitType.Gas:       return BuildingType.Gas;
                case ConduitType.Liquid:    return BuildingType.Liquid;
                case ConduitType.Solid:     return BuildingType.Solid;
                default:                    return BuildingType.None;
            }
        }

        public static bool IsUsePriority(BuildingType buildingType) {
            return buildingType == BuildingType.Gas
                || buildingType == BuildingType.Liquid
                || buildingType == BuildingType.Solid;
        }
    }
}