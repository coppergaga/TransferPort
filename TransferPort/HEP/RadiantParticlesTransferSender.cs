namespace RsTransferPort {
    public class RadiantParticlesTransferSender : StateMachineComponent<RadiantParticlesTransferSender.StatesInstance> {
        public static HashedString PORT_ID = "RadiantParticlesTransferSender";

        [MyCmpGet] private HighEnergyParticleStorage storage;
        [MyCmpGet] private HighEnergyParticlePort port;
        [MyCmpGet] private Operational operational;
        [MyCmpGet] private KSelectable kSelectable;
        [MyCmpGet] private LogicPorts logicPorts;
        [MyCmpGet] private PortItem item;

        public static Operational.Flag receiverFlag = new Operational.Flag("ParticlesTransferSenderFlag", Operational.Flag.Type.Requirement);

        private const float directorDelay = 0.5f;
        private static StatusItem infoStatusItem;

        private bool m_receiverAllow = false;
        /// <summary>
        /// 接收端可用,受频道控制
        /// </summary>
        private bool ReceiverAllow {
            get => m_receiverAllow;
            set {
                m_receiverAllow = value;
                kSelectable.ToggleStatusItem(infoStatusItem, !value);
                operational.SetFlag(receiverFlag, value);
                logicPorts.SendSignal(PORT_ID, value ? 1 : 0);
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
            item.HandleReturnInt = HandleHasRadiation;
            item.HandleReturnFloat = HandleConsumeAll;
            item.HandleInParamInt = HandleReceiverAllow;
            smi.StartSM();
        }

        protected override void OnCleanUp() {
            item.HandleReturnInt = null;
            item.HandleReturnFloat = null;
            item.HandleInParamInt = null;
            base.OnCleanUp();
        }

        private bool OnParticleCaptureAllowed(HighEnergyParticle particle) => ReceiverAllow;
        private int HandleHasRadiation() => RsLib.RsUtil.IntFrom(storage.HasRadiation());
        private float HandleConsumeAll() => storage.ConsumeAll();
        private void HandleReceiverAllow(int allow) => ReceiverAllow = RsLib.RsUtil.BoolFrom(allow);

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
                ready.PlayAnim("on", KAnim.PlayMode.Loop)
                    .TagTransition(GameTags.Operational, inoperational, true)
                    .EventTransition(GameHashes.OnParticleStorageChanged, redirect);
                redirect.PlayAnim("working_pre")
                    .QueueAnim("working_loop")
                    .QueueAnim("working_pst")
                    .ScheduleGoTo(smi => directorDelay, ready);
            }
        }
    }
}