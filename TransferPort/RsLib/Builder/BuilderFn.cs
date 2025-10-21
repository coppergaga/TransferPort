using System;

namespace RsLib.Builder
{
    public class BuilderFn<T> : BaseBuilder<T>
        where T : class
    {
        private Action<BaseBuilder<T>> buildFn;

        public BuilderFn(Action<BaseBuilder<T>> buildFn)
        {
            this.buildFn = buildFn;
        }

        public override void Build()
        {
            buildFn(this);
        }
    }
}