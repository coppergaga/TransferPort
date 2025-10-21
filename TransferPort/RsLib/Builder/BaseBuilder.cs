using System;
using System.Collections.Generic;
using System.Text;

namespace RsLib.Builder
{
    public class BaseBuilder<T> : IBuilder
    where T : class
    {

        public StringBuilder builder;
        public string prefix;
        public T target;
        public IList<OtherBuilderInfo> otherBuilderInfos;
        public virtual bool AllowBuild(object obj)
        {
            return obj is T;
        }
        
        public virtual void Build(object target,StringBuilder builder, string prefix, List<OtherBuilderInfo> otherBuilderInfos)
        {
            this.builder = builder;
            this.prefix = prefix;
            this.target = (T)target;
            this.otherBuilderInfos = otherBuilderInfos;
            try
            {
                Build();
            }
            catch (Exception)
            {
                Debug.LogErrorFormat("build field by {0}",GetType().FullName);
                throw;
            }
            this.builder = null;
            this.prefix = null;
            this.target = null;
            this.otherBuilderInfos = null;
        }
        
        
        
        public virtual void Build()
        {
        }

        public virtual void AppendKeyValue(string key, object value, bool checkAddendSpace = true)
        {
            
            Append(key, checkAddendSpace).Append(":").Append(value ?? "<null>");
            Append(" |");
        }

        public virtual void CheckAddendSpace()
        {
            if (LastChar() != ' ' && builder.Length > 0)
            {
                builder.Append(" ");
            }
        }

        public virtual StringBuilder Append(object text, bool checkAddendSpace = false)
        {
            if (checkAddendSpace)
            {
                CheckAddendSpace();
            }
            return builder.Append(text);
        }
        public virtual StringBuilder AppendPrefix()
        {
            return builder.Append(prefix);
        }
        
        public virtual void AppendLine(string text = "")
        {
            builder.AppendLine(text);
        }

        public virtual char LastChar()
        {
            return builder[builder.Length - 1];
        }
        
        // public virtual void AppendObjectFields()
        // {
        //     return builder[builder.Length - 1];
        // }
    }
}