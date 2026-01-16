using System.Collections.Generic;
using TUNING;
using UnityEngine;
using S_BUILDINGS = STRINGS.BUILDINGS;

namespace RsTransferPort {
    public class WirelessLogicSenderConfig : WirelessLogicConfig {
        public const string ID = "RsWirelessLogicSender";
        public override string id => ID;
        public override string anim => "rs_wireless_logic_sender_kanim";

        public override InOutType InOutType => InOutType.Sender;
        public override string defaultName => STRINGS.BUILDINGS.PREFABS.RSWIRELESSLOGICSENDER.DEFAULTNAME;
    }

    public class WirelessLogicReceiverConfig : WirelessLogicConfig {
        public const string ID = "RsWirelessLogicReceiver";
        public override string id => ID;
        public override string anim => "rs_wireless_logic_receiver_kanim";

        public override InOutType InOutType => InOutType.Receiver;
        public override string defaultName => STRINGS.BUILDINGS.PREFABS.RSWIRELESSLOGICRECEIVER.DEFAULTNAME;
    }

    public abstract class WirelessLogicConfig : IBuildingConfig {
        public const string PORT_ID = "MyPort";

        public abstract string id { get; }
        public abstract string anim { get; }
        public abstract InOutType InOutType { get; }

        public abstract string defaultName { get; }

        public override BuildingDef CreateBuildingDef() {
            var buildingDef = MyUtils.CreateTransferBuildingDef(id, anim, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1,
                MATERIALS.REFINED_METALS);

            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.ObjectLayer = ObjectLayer.LogicGate;
            buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
            // buildingDef.AlwaysOperational = true;
            if (InOutType == InOutType.Sender)
                buildingDef.LogicInputPorts = new List<LogicPorts.Port>
                {
                    LogicPorts.Port.RibbonInputPort((HashedString) PORT_ID, new CellOffset(0, 0),
                        S_BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT,
                        S_BUILDINGS.PREFABS.LOGICINTERASTEROIDSENDER.LOGIC_PORT_ACTIVE,
                        S_BUILDINGS.PREFABS.LOGICINTERASTEROIDSENDER.LOGIC_PORT_INACTIVE, true)
                };
            else
                buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
                {
                    LogicPorts.Port.RibbonOutputPort((HashedString) PORT_ID, new CellOffset(0, 0),
                        S_BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT,
                        S_BUILDINGS.PREFABS.LOGICINTERASTEROIDSENDER.LOGIC_PORT_ACTIVE,
                        S_BUILDINGS.PREFABS.LOGICINTERASTEROIDSENDER.LOGIC_PORT_INACTIVE, true)
                };

            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, id);
            MyOverlayScreen.LogicPortIDs.Add(id);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = Tags.WirelessLogic;
            var channel = go.AddOrGet<PortItem>();
            channel.BuildingType = BuildingType.Logic;
            channel.InOutType = InOutType;

            var port = go.AddOrGet<WirelessLogicPort>();
            port.inOutType = InOutType;
            port.portId = PORT_ID;
            go.AddOrGet<ShowOverlaySelf>();
        }

        public override void DoPostConfigureComplete(GameObject go) {
            Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
        }
    }
}