using System.Collections.Generic;
using UnityEngine;

namespace RsTransferPort
{
    public class RsHashGameObjectPool
    {
        // private Dictionary<object, GameObject> dic = new Dictionary<object, GameObject>();
        // private Dictionary<object, GameObject> dic = new Dictionary<object, GameObject>();

        private readonly Dictionary<object, GameObject> dic = new Dictionary<object, GameObject>();
        private readonly HashSet<object> noRecord = new HashSet<object>();
        private readonly UIGameObjectPool pool;

        public RsHashGameObjectPool(GameObject prefab)
        {
            pool = new UIGameObjectPool(prefab);
        }

        public void RecordStart()
        {
            noRecord.UnionWith(dic.Keys);
        }

        public void ClearNoRecordElement()
        {
            foreach (var key in noRecord)
            {
                GameObject element;
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

        public GameObject GetFreeElement(object key, GameObject instantiateParent = null, bool forceActive = false)
        {
            GameObject element;
            if (!dic.TryGetValue(key, out element))
            {
                element = pool.GetFreeElement(instantiateParent, forceActive);
                dic.Add(key, element);
            }

            noRecord.Remove(key);
            return element;
        }

        public bool GetFreeElement(object key, out GameObject element, GameObject instantiateParent = null,
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

        public void GetFreeElement(object key, out GameObject element, out bool isNew, GameObject instantiateParent = null,
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