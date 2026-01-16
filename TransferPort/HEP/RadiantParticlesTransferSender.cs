using KSerialization;

namespace RsTransferPort {
    public class RadiantParticlesTransferSender :
        StateMachineComponent<RadiantParticlesTransferSender.StatesInstance> {
        public static HashedString PORT_ID = "RadiantParticlesTransferSender";

        [MyCmpReq]
        private KSelectable selectable;
        [MyCmpReq]
        public HighEnergyParticleStorage storage;
        [MyCmpReq]
        private HighEnergyParticlePort port;
        [MyCmpReq]
        private LogicPorts logicPorts;
        [MyCmpReq]
        private Operational operational;

        [Serialize] private EightDirection _direction;

        private EightDirectionController directionController;

        public float directorDelay = 0.5f;

        private bool hasLogicWire;

        private static StatusItem infoStatusItem;

        public static Operational.Flag receiverFlag = new Operational.Flag("ParticlesTransferSenderFlag", Operational.Flag.Type.Requirement);

        private bool m_receiverAllow = false;

        /// <summary>
        /// 接收端可用,受频道控制
        /// </summary>
        public bool ReceiverAllow {
            get => m_receiverAllow;
            set {
                m_receiverAllow = value;
                GetComponent<KSelectable>().ToggleStatusItem(infoStatusItem, !value);
                operational.SetFlag(receiverFlag, value);
                GetComponent<LogicPorts>().SendSignal(PORT_ID, value ? 1 : 0);
            }
        }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            if (infoStatusItem == null) {
                infoStatusItem = new StatusItem("RsRadiantParticlesTransferSenderInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            }
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            port.onParticleCaptureAllowed += OnParticleCaptureAllowed;
            smi.StartSM();
        }

        private bool OnParticleCaptureAllowed(HighEnergyParticle particle) => ReceiverAllow;

        public class StatesInstance :
            GameStateMachine<States, StatesInstance, RadiantParticlesTransferSender, object>.GameInstance {
            public StatesInstance(RadiantParticlesTransferSender smi)
                : base(smi) {
            }
        }

        public class States :
            GameStateMachine<States, StatesInstance,
                RadiantParticlesTransferSender> {
            public State inoperational;
            public State ready;
            public State redirect;

            public override void InitializeStates(out BaseState default_state) {
                default_state = inoperational;
                inoperational.PlayAnim("off").TagTransition(GameTags.Operational, ready);
                ready.PlayAnim("on", KAnim.PlayMode.Loop).TagTransition(GameTags.Operational, inoperational, true)
                    .EventTransition(GameHashes.OnParticleStorageChanged, redirect);
                redirect.PlayAnim("working_pre").QueueAnim("working_loop").QueueAnim("working_pst")
                    .ScheduleGoTo(smi => smi.master.directorDelay, ready);
            }
        }
    }
}