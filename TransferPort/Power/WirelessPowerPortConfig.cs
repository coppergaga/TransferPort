using TUNING;
using UnityEngine;

namespace RsTransferPort
{
    // public class PortalPowerSenderConfig : PortalPowerConfig
    // {
    //     public static string ID = "PortalPowerSender";
    //     public static string Anim = "portal_power_sender_kanim";
    //     public PortalPowerSenderConfig() : base(id, anim, conduitType, portalType)
    //     {
    //     }
    // }

    /// <summary>
    ///     电力传送的基类
    /// </summary>
    public class WirelessPowerPortConfig : IBuildingConfig
    {
        public const string ID = "RsWirelessPowerPort";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = MyUtils.CreateTransferBuildingDef(
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

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            var channel = go.AddOrGet<PortItem>();
            channel.BuildingType = BuildingType.Power;
            channel.InOutType = InOutType.None;
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = Tags.WirelessPower;
            go.AddOrGet<WirelessPowerPort>();
            go.AddOrGet<ShowOverlaySelf>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
        }
    }
}