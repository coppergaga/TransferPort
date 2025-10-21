using System;
using System.Collections.Generic;

namespace RsLib.Collections
{
    public class ContrastSet<T>
    {
        private Action<T> onAdd;
        private Action<T> onRemove;

        private HashSet<T> source = new HashSet<T>();
        private HashSet<T> target = new HashSet<T>();
        
        public ContrastSet(Action<T> onAdd, Action<T> onRemove)
        {
            this.onAdd = onAdd;
            this.onRemove = onRemove;
        }

        public void Add(T obj)
        {
            source.Add(obj);
        }

        public void Remove(T obj)
        {
            source.Remove(obj);
        }

        public void StartRecord()
        {
            source.Clear();
        }

        public void EndAndContrast()
        {
            RsUtil.ContrastSet(source, target, onAdd, onRemove);
            source.Clear();
        }

        public void Clear()
        {
            StartRecord();
            EndAndContrast();
        }
    }
}