using TUNING;
using UnityEngine;

namespace RsTransferPort
{
    public class TransferPortCenterConfig : IBuildingConfig
    {
        public const string ID = "RsTransferPortCenter";
        
        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = global::BuildingTemplates.CreateBuildingDef(
                ID,
                4,
                4,
                "rs_transfer_port_center_kanim",
                1000,
                15,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER5,
                MATERIALS.REFINED_METALS,
                2400f,
                BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.BONUS.TIER5,
                NOISE_POLLUTION.NONE
            );
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.DefaultAnimState = "working_loop";
            // buildingDef.ViewMode = MyOverlayMods.PortChannel.ID;
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddComponent<TransferPortCenter>();
            Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
        }
        
    }
}