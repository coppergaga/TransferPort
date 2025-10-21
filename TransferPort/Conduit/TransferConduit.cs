
namespace RsTransferPort
{
    public class TransferConduit : KMonoBehaviour
    {
        [MyCmpGet] public Building building;
        [MyCmpGet] public TransferPortChannel channelItem;
        
        /// <summary>
        ///     管道类型
        /// </summary>
        public ConduitType conduitType = ConduitType.None;

        public InOutType inOutType;
        
        protected override void OnCleanUp()
        {
        }

        public int GetCell()
        {
            if (inOutType == InOutType.Receiver) return building.GetUtilityOutputCell();
            return building.GetUtilityInputCell();
        }
    }
}