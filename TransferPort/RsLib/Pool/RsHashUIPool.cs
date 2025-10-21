using System.Collections.Generic;
using UnityEngine;

namespace RsTransferPort
{
    public class RsHashUIPool<T> where T : MonoBehaviour
    {
        // private Dictionary<object, T> dic = new Dictionary<object, T>();
        // private Dictionary<object, T> dic = new Dictionary<object, T>();

        private readonly Dictionary<object, T> dic = new Dictionary<object, T>();
        private readonly HashSet<object> noRecord = new HashSet<object>();
        private readonly UIPool<T> pool;

        public RsHashUIPool(T prefab)
        {
            pool = new UIPool<T>(prefab);
        }

        public void RecordStart()
        {
            noRecord.UnionWith(dic.Keys);
        }

        public void ClearNoRecordElement()
        {
            foreach (var key in noRecord)
            {
                T element;
                if (dic.TryGetValue(key, out element))
                {
                    pool.ClearElement(element);
                    dic.Remove(key);
                }
            }

            noRecord.Clear();
        }

        public bool HasElement(object key)
        {
            return dic.ContainsKey(key);
        }

        public void RecordElement(object key)
        {
            noRecord.Remove(key);
        }

        public T GetFreeElement(object key, GameObject instantiateParent = null, bool forceActive = false)
        {
            T element;
            if (!dic.TryGetValue(key, out element))
            {
                element = pool.GetFreeElement(instantiateParent, forceActive);
                dic.Add(key, element);
            }

            noRecord.Remove(key);
            return element;
        }

        public bool GetFreeElement(object key, out T element, GameObject instantiateParent = null,
            bool forceActive = false)
        {
            var isNew = false;
            if (!dic.TryGetValue(key, out element))
            {
                element = pool.GetFreeElement(instantiateParent, forceActive);
                dic.Add(key, element);
                isNew = true;
            }

            noRecord.Remove(key);
            return isNew;
        }

        public void GetFreeElement(object key, out T element, out bool isNew, GameObject instantiateParent = null,
            bool forceActive = false)
        {
            if (isNew = !dic.TryGetValue(key, out element))
            {
                element = pool.GetFreeElement(instantiateParent, forceActive);
                dic.Add(key, element);
            }

            noRecord.Remove(key);
        }


        public void ClearAll()
        {
            dic.Clear();
            noRecord.Clear();
            pool.ClearAll();
        }
    }
}