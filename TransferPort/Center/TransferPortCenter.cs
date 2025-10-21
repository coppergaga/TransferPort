namespace RsTransferPort
{
    public class TransferPortCenter : StateMachineComponent<TransferPortCenter.StatesInstance>
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            // PortManager.Instance.AddCenter(this);
            smi.StartSM();
        }

        protected override void OnCleanUp()
        {
            // PortManager.Instance.RemoveCenter(this);
            base.OnCleanUp();
        }
        
        
        public class StatesInstance :
            GameStateMachine<States, StatesInstance, TransferPortCenter, object>.GameInstance
        {
            public StatesInstance(TransferPortCenter smi)
                : base(smi)
            {
            }
        }

        public class States :
            GameStateMachine<States, StatesInstance,
                TransferPortCenter>
        {
            public State workingLoop;
            public override void InitializeStates(out BaseState default_state)
            {
                default_state = workingLoop;
                workingLoop.PlayAnim("working_loop", KAnim.PlayMode.Loop);
            }
        }
        
    }
}