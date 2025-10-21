using System.Collections.Generic;
using System.Text;

namespace RsLib.Builder
{
    public class BuilderManager : List<IBuilder>
    {
        private StringBuilder stringBuilder = new StringBuilder();
        List<IBuilder> FindAllowBuilder(object obj)
        {
            List<IBuilder> list = new List<IBuilder>();
            foreach (IBuilder builder in this)
            {
                if (builder.AllowBuild(obj))
                {
                    list.Add(builder);
                }
            }
            return  list;
        }

        public void Build(object obj, string prefix = "")
        {
            List<OtherBuilderInfo> builderInfos = new List<OtherBuilderInfo>();
            foreach (IBuilder builder in this)
            {
                if (builder.AllowBuild(obj))
                {
                    builder.Build(obj, stringBuilder, prefix, builderInfos);
                }
            }

            foreach (OtherBuilderInfo builderInfo in builderInfos)
            {
                stringBuilder.AppendLine();
                Build(builderInfo.target, builderInfo.prefix);
            }
        }
        
        public string BuildToString(object obj, string prefix = "", bool newLine = false)
        {
            stringBuilder.Clear();
            if (newLine)
            {
                stringBuilder.AppendLine();
            }
            Build(obj, prefix);
            return stringBuilder.ToString();
        }
        
    }
}