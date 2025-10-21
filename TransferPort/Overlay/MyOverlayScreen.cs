using System.Collections.Generic;

namespace RsTransferPort
{
    public class MyOverlayScreen
    {
        public static HashSet<Tag> LiquidPortIDs = new HashSet<Tag>();
        public static HashSet<Tag> SolidPortDs = new HashSet<Tag>();
        public static HashSet<Tag> GasPortIDs = new HashSet<Tag>();
        public static HashSet<Tag> PowerPortIDs = new HashSet<Tag>();
        public static HashSet<Tag> LogicPortIDs = new HashSet<Tag>();

        private static readonly SampleLazy<HashSet<Tag>> _AllPortIDs = new SampleLazy<HashSet<Tag>>(() =>
        {
            HashSet<Tag> tags = new HashSet<Tag>();
            tags.UnionWith(LiquidPortIDs);
            tags.UnionWith(SolidPortDs);
            tags.UnionWith(GasPortIDs);
            tags.UnionWith(PowerPortIDs);
            tags.UnionWith(LogicPortIDs);
            return tags;
        });

        public static HashSet<Tag> AllPortIDs => _AllPortIDs.Value;
    }
}