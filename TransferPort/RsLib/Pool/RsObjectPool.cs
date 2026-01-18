using System;
using System.Collections.Generic;

namespace RsLib.Pool {
    public class RsObjectPool<T> {
        private readonly Stack<T> pool = new Stack<T>();
        private readonly Func<T> createFn;
        private readonly Action<T> releaseFn;

        public RsObjectPool(Func<T> createFn, Action<T> releaseFn) {
            this.createFn = createFn;
            this.releaseFn = releaseFn;
        }

        public T Get() {
            if (!pool.TryPop(out T last)) {
                last = createFn();
                pool.Push(last);
            }

            return last;
        }

        public void Release(T t) {
            pool.Push(t);
            releaseFn(t);
        }
    }
}

public static class StackExtensions {
    public static bool TryPop<T>(this Stack<T> stack, out T result) {
        if (stack.Count > 0) {
            result = stack.Pop();
            return true;
        }
        result = default;
        return false;
    }
}
