using System.Collections.Generic;

namespace RsLib.Pool
{
    public class RsListPool<T> : RsObjectPool<List<T>>
    {
        public RsListPool() : base(() => new List<T>(), t => t.Clear())
        {
        }
    }
}