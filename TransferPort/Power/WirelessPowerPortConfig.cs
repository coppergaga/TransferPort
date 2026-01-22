using TUNING;
using UnityEngine;

namespace RsTransferPort {
    /// <summary>
    ///     电力传送的基类
    /// </summary>
    public class WirelessPowerPortConfig : IBuildingConfig {
        public const string ID = "RsWirelessPowerPort";

        public override BuildingDef CreateBuildingDef() {
            var buildingDef = MyUtils.BaseBuildingDef(
                ID,
                "rs_wireless_power_port_Kanim",
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                MATERIALS.REFINED_METALS);

            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.ObjectLayer = ObjectLayer.WireConnectors;
            buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;

            buildingDef.RequiresPowerOutput = true;
            buildingDef.PowerOutputOffset = new CellOffset(0, 0);
            buildingDef.UseWhitePowerOutputConnectorColour = true;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, ID);
            MyOverlayScreen.PowerPortIDs.Add(ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            var item = go.AddOrGet<PortItem>();
            item.BuildingType = BuildingType.Power;
            item.InOutType = InOutType.None;
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = Tags.WirelessPower;
            go.AddOrGet<WirelessPowerPort>();
            go.AddOrGet<ShowOverlaySelf>();
        }

        public override void DoPostConfigureComplete(GameObject go) {
            Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
        }
    }
}