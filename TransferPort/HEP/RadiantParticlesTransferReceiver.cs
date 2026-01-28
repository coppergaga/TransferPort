using KSerialization;
using UnityEngine;

namespace RsTransferPort {
    public class RadiantParticlesTransferReceiver : StateMachineComponent<RadiantParticlesTransferReceiver.StatesInstance>, IMyHighEnergyParticleDirection {
        // public static HashedString PORT_ID = "RadiantParticlesTransferReceiver";
        [MyCmpGet] private Operational operational;
        [MyCmpGet] private KBatchedAnimController animController;
        [MyCmpGet] private PortItem item;
        [MyCmpGet] private HighEnergyParticleStorage hepStorage;
        [MyCmpGet] private HighEnergyParticlePort hepPort;
        [MyCmpGet] private Building building;

        [Serialize] private EightDirection _direction;
        private EightDirectionController directionController;

        public const float directorDelay = 0.5f;

        /// <summary>
        /// 是否可用传送
        /// </summary>
        private int Transmissible() => RsLib.RsUtil.IntFrom(operational != null && operational.IsOperational);

        public EightDirection Direction {
            get => _direction;
            set {
                _direction = value;
                if (Util.IsNullOrDestroyed(directionController)) { return; }
                directionController.SetRotation(45 * EightDirectionUtil.GetDirectionIndex(_direction));
                if (Util.IsNullOrDestroyed(directionController.controller)) { return; }
                directionController.controller.SetDirty();
            }
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            // Subscribe((int) GameHashes.OperationalChanged, OnOperationalChanged);
            directionController = new EightDirectionController(
                animController, "redirector_target", "redirector_off", EightDirectionController.Offset.Infront
            );
            Direction = Direction;
            item.HandleReturnInt = Transmissible;
            item.HandleInParamFloat = StoreAndLaunch;
            smi.StartSM();
        }

        protected override void OnCleanUp() {
            if (!Util.IsNullOrDestroyed(item)) {
                item.HandleReturnInt = null;
                item.HandleInParamFloat = null;
            }
            base.OnCleanUp();
        }

        private void StoreAndLaunch(float amount) {
            if (amount <= 0) { return; }
            hepStorage.Store(amount);
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, RadiantParticlesTransferReceiver, StateMachine.BaseDef>.GameInstance {
            public StatesInstance(RadiantParticlesTransferReceiver smi) : base(smi) { }

            public void LaunchParticle() {
                if (master.hepStorage.Particles < 0.100000001490116) {
                    master.hepStorage.ConsumeAll();
                    return;
                }
                int particleOutputCell = master.building.GetHighEnergyParticleOutputCell();
                GameObject go = GameUtil.KInstantiate(
                    global::Assets.GetPrefab((Tag)"HighEnergyParticle"),
                    Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2),
                    Grid.SceneLayer.FXFront2
                );
                go.SetActive(true);

                if (Util.IsNullOrDestroyed(go)) { return; }

                HighEnergyParticle hep = go.GetComponent<HighEnergyParticle>();
                hep.payload = master.hepStorage.ConsumeAll();
                hep.payload -= 0.1f;
                hep.capturedBy = master.hepPort;
                hep.SetDirection(master.Direction);
                master.directionController.PlayAnim("redirector_send");
            }
        }

        public class States : GameStateMachine<States, StatesInstance, RadiantParticlesTransferReceiver, StateMachine.BaseDef> {
            public State off;
            public State ready;
            public State launch;

            public override void InitializeStates(out BaseState default_state) {
                default_state = off;
                off.PlayAnim("off")
                    .Enter(smi => smi.master.directionController.PlayAnim("redirector_off"))
                    .TagTransition(GameTags.Operational, ready);

                ready.PlayAnim("on")
                    .Enter(smi => smi.master.directionController.PlayAnim("redirector_on", KAnim.PlayMode.Loop))
                    .TagTransition(GameTags.Operational, off, true)
                    .UpdateTransition(launch, (smi, dt) => !smi.master.hepStorage.IsEmpty(), UpdateRate.SIM_200ms);
                // .EventTransition(GameHashes.OnParticleStorageChanged, launch);
                // launch_pre.PlayAnim("on").ScheduleGoTo(smi => smi.master.directorDelay, launch);
                launch.PlayAnim("on")
                    .Enter(smi => smi.LaunchParticle())
                    .ScheduleGoTo(smi => directorDelay, ready);
            }
        }
    }
}