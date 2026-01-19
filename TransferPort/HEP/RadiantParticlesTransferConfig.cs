using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace RsTransferPort {

    public class RadiantParticlesTransferSenderConfig : RadiantParticlesTransferConfig {
        public const string ID = "RsRadiantParticlesTransferSender";
        private const string Anim = "rs_radiant_particles_transfer_sender_kanim";
        public RadiantParticlesTransferSenderConfig() : base(ID, Anim, InOutType.Sender) { }
    }

    public class RadiantParticlesTransferReceiverConfig : RadiantParticlesTransferConfig {
        public const string ID = "RsRadiantParticlesTransferReceiver";
        private const string Anim = "rs_radiant_particles_transfer_receiver_kanim";

        public RadiantParticlesTransferReceiverConfig() : base(ID, Anim, InOutType.Receiver) { }
    }


    /// <summary>
    /// 辐射粒子
    /// </summary>
    public abstract class RadiantParticlesTransferConfig : IBuildingConfig {
        protected string id;
        protected string anim;
        protected InOutType inOutType;

        public RadiantParticlesTransferConfig(string id, string anim, InOutType inOutType) {
            this.id = id;
            this.anim = anim;
            this.inOutType = inOutType;
        }

        public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

        public override BuildingDef CreateBuildingDef() {
            BuildingDef buildingDef =
                MyUtils.BaseBuildingDef(id, anim, BUILDINGS.CONSTRUCTION_MASS_KG.TIER6,
                    MATERIALS.RAW_MINERALS);

            buildingDef.DefaultAnimState = "off";

            if (inOutType == InOutType.Sender) {
                buildingDef.UseHighEnergyParticleInputPort = true;
                buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 0);
                buildingDef.LogicOutputPorts = new List<LogicPorts.Port> {
                    LogicPorts.Port.OutputPort(
                        RadiantParticlesTransferSender.PORT_ID, new CellOffset(0, 0),
                        STRINGS.BUILDINGS.PREFABS.RSRADIANTPARTICLESTRANSFERSENDER.LOGIC_PORT.ToString(),
                        STRINGS.BUILDINGS.PREFABS.RSRADIANTPARTICLESTRANSFERSENDER.LOGIC_PORT_ACTIVE.ToString(),
                        STRINGS.BUILDINGS.PREFABS.RSRADIANTPARTICLESTRANSFERSENDER.LOGIC_PORT_INACTIVE.ToString()
                    )
                };
            }
            else {
                buildingDef.UseHighEnergyParticleOutputPort = true;
                buildingDef.HighEnergyParticleOutputOffset = new CellOffset(0, 0);
                buildingDef.LogicInputPorts = new List<LogicPorts.Port> {
                    LogicPorts.Port.InputPort(
                        LogicOperationalController.PORT_ID, new CellOffset(0, 0),
                        STRINGS.BUILDINGS.PREFABS.RSRADIANTPARTICLESTRANSFERRECEIVER.LOGIC_PORT.ToString(),
                        STRINGS.BUILDINGS.PREFABS.RSRADIANTPARTICLESTRANSFERRECEIVER.LOGIC_PORT_ACTIVE.ToString(),
                        STRINGS.BUILDINGS.PREFABS.RSRADIANTPARTICLESTRANSFERRECEIVER.LOGIC_PORT_INACTIVE.ToString()
                    )
                };
            }

            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, id);
            buildingDef.Deprecated = !Sim.IsRadiationEnabled();
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {

            go.AddOrGet<ShowOverlaySelf>();
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = Tags.TransferRadianParticles;

            HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            energyParticleStorage.autoStore = true;
            energyParticleStorage.showInUI = true;
            energyParticleStorage.capacity = 501f;

            if (inOutType == InOutType.Sender) {
                go.AddOrGet<RadiantParticlesTransferSender>();
                go.AddOrGet<Operational>().SetFlag(RadiantParticlesTransferSender.receiverFlag, false);
            }
            else {
                go.AddOrGet<LogicOperationalController>();
                go.AddOrGet<RadiantParticlesTransferReceiver>();
                go.AddOrGet<Operational>().SetFlag(LogicOperationalController.LogicOperationalFlag, false);
            }

            PortItem channel = go.AddOrGet<PortItem>();
            channel.BuildingType = BuildingType.HEP;
            channel.InOutType = inOutType;
        }

        public override void DoPostConfigureComplete(GameObject go) {
            Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
        }
    }
}