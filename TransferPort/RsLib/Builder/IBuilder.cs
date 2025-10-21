using System.Collections.Generic;
using System.Text;

namespace RsLib.Builder
{
    public interface IBuilder
    {
        bool AllowBuild(object obj);
        void Build(object target, StringBuilder builder, string prefix, List<OtherBuilderInfo> otherBuilderInfos);
        
    }
}