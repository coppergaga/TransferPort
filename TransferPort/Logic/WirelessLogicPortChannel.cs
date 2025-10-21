
namespace RsTransferPort
{
    public class WirelessLogicPortChannel : SingleChannelController
    {
        protected override void OnAdd(TransferPortChannel port)
        {

            if (IsInvalid())
            {
                return;
            }
            
            if (port.InOutType == InOutType.Sender)
            {
                port.Subscribe((int) GameHashes.LogicEvent, OnInputLogicEvent);
            }
            SyncSignal();
        }

        protected override void OnRemove(TransferPortChannel port)
        {
            if (IsInvalid())
            {
                return;
            }
            
            if (port.InOutType == InOutType.Sender)
            {
                port.Unsubscribe((int) GameHashes.LogicEvent, OnInputLogicEvent);
            }

            if (port.InOutType == InOutType.Receiver)
            {
                port.GetComponent<WirelessLogicPort>().SendSignal(0);
            }
            SyncSignal();
        }

        public void OnInputLogicEvent(object data)
        {
            SyncSignal();
        }

        /// <summary>
        ///     输入逻辑信号改变
        /// </summary>
        public void SyncSignal()
        {
            //开始同步信号
            if (receivers.Count == 0) return;
            var signal = GetSignal();
            foreach (TransferPortChannel receiver in receivers) receiver.GetComponent<WirelessLogicPort>().SendSignal(signal);
        }

        /// <summary>
        ///     通过计算所有输入端的信号状态，返回当前通道的信号状态
        /// </summary>
        /// <returns></returns>
        public int GetSignal()
        {
            int signal = 0;

            foreach (TransferPortChannel sender in senders)
            {
                int inputSignal = sender.GetComponent<WirelessLogicPort>().GetInputSignal();
                signal = signal | inputSignal;
            }
            
            return signal;
        }

     


        public WirelessLogicPortChannel(BuildingType buildingType, string channelName, int worldIdAG) : base(buildingType, channelName, worldIdAG)
        {
        }
    }
}