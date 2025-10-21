namespace RsLib.Builder
{
    public struct OtherBuilderInfo
    {
        public string prefix;
        public object target;

        public OtherBuilderInfo(string prefix, object target)
        {
            this.prefix = prefix;
            this.target = target;
        }
    }
}