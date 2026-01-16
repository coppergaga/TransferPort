using TUNING;
using UnityEngine;

namespace RsTransferPort
{
    public class LiquidTransferConduitSenderConfig : TransferConduitConfig
    {
        public static string ID = "RsLiquidTransferConduitSender";
        public static string Anim = "rs_liquid_transfer_conduit_sender_kanim";

        public LiquidTransferConduitSenderConfig() : base(ID, Anim, ConduitType.Liquid, InOutType.Sender)
        {
        }

        public override string defaultName => STRINGS.BUILDINGS.PREFABS.RSLIQUIDTRANSFERCONDUITSENDER.DEFAULTNAME;
    }

    public class LiquidTransferConduitReceiverConfig : TransferConduitConfig
    {
        public static string ID = "RsLiquidTransferConduitReceiver";
        public static string Anim = "rs_liquid_transfer_conduit_receiver_kanim";

        public LiquidTransferConduitReceiverConfig() : base(ID, Anim, ConduitType.Liquid, InOutType.Receiver)
        {
        }

        public override string defaultName => STRINGS.BUILDINGS.PREFABS.RSLIQUIDTRANSFERCONDUITRECEIVER.DEFAULTNAME;
    }

    public class GasTransferConduitSenderConfig : TransferConduitConfig
    {
        public static string ID = "RsGasTransferConduitSender";
        public static string Anim = "rs_gas_transfer_conduit_sender_kanim";

        public GasTransferConduitSenderConfig() : base(ID, Anim, ConduitType.Gas, InOutType.Sender)
        {
        }

        public override string defaultName => STRINGS.BUILDINGS.PREFABS.RSGASTRANSFERCONDUITSENDER.DEFAULTNAME;
    }

    public class GasTransferConduitReceiverConfig : TransferConduitConfig
    {
        public static string ID = "RsGasTransferConduitReceiver";
        public static string Anim = "rs_gas_transfer_conduit_Receiver_kanim";

        public GasTransferConduitReceiverConfig() : base(ID, Anim, ConduitType.Gas, InOutType.Receiver)
        {
        }

        public override string defaultName => STRINGS.BUILDINGS.PREFABS.RSGASTRANSFERCONDUITRECEIVER.DEFAULTNAME;
    }

    public class SolidTransferConduitSenderConfig : TransferConduitConfig
    {
        public static string ID = "RsSolidTransferConduitSender";
        public static string Anim = "rs_solid_transfer_conduit_sender_kanim";

        public SolidTransferConduitSenderConfig() : base(ID, Anim, ConduitType.Solid, InOutType.Sender)
        {
        }

        public override string defaultName => STRINGS.BUILDINGS.PREFABS.RSSOLIDTRANSFERCONDUITSENDER.DEFAULTNAME;
    }

    public class SolidTransferConduitReceiverConfig : TransferConduitConfig
    {
        public static string ID = "RsSolidTransferConduitReceiver";
        public static string Anim = "rs_solid_transfer_conduit_receiver_kanim";

        public SolidTransferConduitReceiverConfig() : base(ID, Anim, ConduitType.Solid, InOutType.Receiver)
        {
        }

        public override string defaultName => STRINGS.BUILDINGS.PREFABS.RSSOLIDTRANSFERCONDUITRECEIVER.DEFAULTNAME;
    }


    /// <summary>
    ///     气体 液体 固体 传送管道的基类
    /// </summary>
    public abstract class TransferConduitConfig : IBuildingConfig
    {
        protected string anim;
        protected ConduitType conduitType;
        protected string id;
        protected InOutType inOutType;

        protected TransferConduitConfig(string id, string anim, ConduitType conduitType, InOutType inOutType)
        {
            this.id = id;
            this.anim = anim;
            this.conduitType = conduitType;
            this.inOutType = inOutType;
        }

        public virtual string defaultName { get; }

        public override BuildingDef CreateBuildingDef()
        {
            var refined_METALS = MATERIALS.RAW_MINERALS; //建造材料
            switch (conduitType)
            {
                case ConduitType.Liquid:
                    refined_METALS = MATERIALS.RAW_MINERALS;
                    break;
                case ConduitType.Gas:
                    refined_METALS = MATERIALS.RAW_MINERALS;
                    break;
                case ConduitType.Solid:
                    refined_METALS = MATERIALS.RAW_METALS;
                    break;
            }

            var buildingDef = MyUtils.CreateTransferBuildingDef(
                id,
                anim,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER6,
                refined_METALS);

            if (conduitType == ConduitType.Liquid)
            {
                buildingDef.ViewMode = OverlayModes.LiquidConduits.ID; //视图显示
                buildingDef.ObjectLayer = ObjectLayer.LiquidConduitConnection; //建造图层
                buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduitBridges; //场景渲染图层
                GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, id); //视图凸显
                MyOverlayScreen.LiquidPortIDs.Add(id);
            }
            else if (conduitType == ConduitType.Gas)
            {
                buildingDef.ViewMode = OverlayModes.GasConduits.ID; //视图显示
                buildingDef.ObjectLayer = ObjectLayer.GasConduitConnection; //建造图层
                buildingDef.SceneLayer = Grid.SceneLayer.GasConduitBridges; //场景渲染图层
                GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, id); //视图凸显
                MyOverlayScreen.GasPortIDs.Add(id);
            }
            else if (conduitType == ConduitType.Solid)
            {
                buildingDef.ViewMode = OverlayModes.SolidConveyor.ID; //视图显示
                buildingDef.ObjectLayer = ObjectLayer.SolidConduitConnection; //建造图层
                buildingDef.SceneLayer = Grid.SceneLayer.SolidConduitBridges; //场景渲染图层
                GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, id); //视图凸显
                MyOverlayScreen.SolidPortDs.Add(id);
            }


            if (inOutType == InOutType.Sender)
            {
                buildingDef.InputConduitType = conduitType; //输入管道类型
                buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            }
            else
            {
                buildingDef.OutputConduitType = conduitType; //输出管道类型
                buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            }

            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
            Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
            Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
            Object.DestroyImmediate(go.GetComponent<RequireInputs>());

            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = Tags.TransferConduit;
            var channel = go.AddOrGet<PortItem>();
            channel.BuildingType = Converter.FromConduitType(conduitType);
            channel.InOutType = inOutType;

            go.AddOrGet<ShowOverlaySelf>();
            
            var TransferConduit = go.AddOrGet<TransferConduit>();
            TransferConduit.inOutType = inOutType;
            TransferConduit.conduitType = conduitType;
        }
    }
}