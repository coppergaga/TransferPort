using System;
using System.Threading;

namespace RsTransferPort {
    public class SampleLazy<T> where T : class {
        private Lazy<T> m_lazyInst;
        private readonly Func<T> m_valueFactory;
        private readonly LazyThreadSafetyMode m_threadSafetyMode;
        private readonly object m_lock = new object();

        public SampleLazy(
            Func<T> valueFactory,
            bool preLoadSceneClear = false,
            LazyThreadSafetyMode mode = LazyThreadSafetyMode.ExecutionAndPublication) {

            m_valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            m_threadSafetyMode = mode;
            CreateNewLazy();
            if (preLoadSceneClear) App.OnPreLoadScene += ClearValue;
        }

        public T Value => m_lazyInst.Value;
        public bool IsValueCreated => m_lazyInst.IsValueCreated;

        public void ClearValue() {
            // 防止在高并发下，多个线程同时重置导致 Lazy 实例不一致
            lock (m_lock) { CreateNewLazy(); }
        }

        private void CreateNewLazy() {
            m_lazyInst = new Lazy<T>(m_valueFactory, m_threadSafetyMode);
        }
    }
}