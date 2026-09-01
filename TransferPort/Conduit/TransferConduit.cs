
namespace RsTransferPort {
    public class TransferConduit : KMonoBehaviour {
        [MyCmpGet] public Building building;
        public InOutType InOutTypo;

        public int ConduitIOCell {
            get {
                if (int.MinValue == _posCache) {
                    _posCache = GetCell();
                }
                return _posCache;
            }
        }
        private int _posCache = int.MinValue;
        private int GetCell() {
            if (InOutTypo == InOutType.Receiver) return building.GetUtilityOutputCell();
            return building.GetUtilityInputCell();
        }
    }
}