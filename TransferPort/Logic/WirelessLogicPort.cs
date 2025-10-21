
namespace RsTransferPort
{
    public class WirelessLogicPort : KMonoBehaviour
    {
        [MyCmpGet] public TransferPortChannel channelItem;
        public InOutType inOutType;
        public HashedString portId;

        public virtual int GetInputSignal()
        {
            return GetComponent<LogicPorts>().GetInputValue(portId);
        }

        public virtual void SendSignal(int signal)
        {
            GetComponent<LogicPorts>().SendSignal(portId, signal);
        }
    }
}