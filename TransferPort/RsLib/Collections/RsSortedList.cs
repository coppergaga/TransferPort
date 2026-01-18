using System;
using System.Collections;
using System.Collections.Generic;

namespace RsLib.Collections {
    public class RsSortedList<T> : ICollection<T> where T : IComparable<T> {
        private readonly List<T> _list = new List<T>();
        public RsSortedList() { }

        public T this[int index] => _list[index];

        public IEnumerator<T> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(T item) {
            if (item == null) { return; }
            if (_list.Count < 1) {
                _list.Add(item);
                return;
            }
            bool isInserted = false;
            for (int i = 0; i < _list.Count; i++) {
                if (item.CompareTo(_list[i]) <= 0) {
                    _list.Insert(i, item);
                    isInserted = true;
                    break;
                }
            }
            if (!isInserted) {
                _list.Add(item);
            }
        }

        public void Clear() {
            _list.Clear();
        }

        public bool Contains(T item) {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            return _list.Remove(item);
        }

        public int Count => _list.Count;
        public bool IsReadOnly => false;
    }
}