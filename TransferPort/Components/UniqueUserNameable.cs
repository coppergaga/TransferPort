namespace RsTransferPort
{
    public class UniqueUserNameable : UserNameable
    {
        public string firstName = "";

        protected override void OnSpawn()
        {
            // Subscribe((int) GameHashes.BuildingStateChanged)
            if (!string.IsNullOrEmpty(savedName))
                SetName(savedName);
            else if (!string.IsNullOrEmpty(firstName))
                SetName(MyUtils.UniqueSaveName(firstName));
            else
                SetName(gameObject.GetProperName());
        }
    }
}