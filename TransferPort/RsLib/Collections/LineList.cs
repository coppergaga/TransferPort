using System;
using System.Collections;
using System.Collections.Generic;

namespace RsLib.Collections
{
    public class LineList<T> : IEnumerable<List<T>>
    {
        protected int idx = -1;

        protected List<List<T>> list = new List<List<T>>();

        protected List<T> currentLine => list[idx];

        public IEnumerator<List<T>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void NextLine()
        {
            idx++;
            if (idx == list.Count) list.Add(new List<T>());
        }

        public void PreviousLineIfEmpty()
        {
            if (idx > -1 && list[idx].Count == 0) idx--;
        }

        public void Add(T t)
        {
            if (idx == -1) throw new Exception("There are currently no rows, call the Next method first.");
            currentLine.Add(t);
            // onAdd?.Invoke(t);
        }

        public void Clear()
        {
            if (idx == -1) return;

            for (var i = 0; i <= idx; i++)
            {
                // if (onRemove != null)
                // {
                //     foreach (T x1 in list[i])
                //         onRemove(x1);
                // }
                list[i].Clear();
            }

            idx = -1;
        }
    }

}