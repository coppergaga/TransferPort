
namespace RsTransferPort {
    public class TransferConduit : KMonoBehaviour {
        [MyCmpGet] public Building building;
        [MyCmpGet] public PortItem item;

        protected override void OnSpawn() {
            base.OnSpawn();
            item.HandleReturnInt = GetCell;
        }

        protected override void OnCleanUp() {
            if (!Util.IsNullOrDestroyed(item)) {
                item.HandleReturnInt = null;
            }
            base.OnCleanUp();
        }

        private int GetCell() {
            if (item.InOutType == InOutType.Receiver) return building.GetUtilityOutputCell();
            return building.GetUtilityInputCell();
        }
    }
}