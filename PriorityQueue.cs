using Global.BusinessCommon.Helpers.Containers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.BusinessCommon.Helpers.Containers
{
    public class PriorityQueue<T> : IQueue<T>, ICollection<T>
    {
        public int Count => _bag.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// O(log n)
        /// </summary>
        public void Add(T item)
        {
            if(_bag.Count <= 0)
            {
                _bag.Add(item);
                return;
            }
            var index = LowerBound(item);
            _bag.Insert(index, item);
        }

        private int LowerBound(T item)
        {
            var analyzingAmount = _bag.Count;
            int first = 0;

            while (analyzingAmount > 0)
            {
                int it = first;
                int step = analyzingAmount / 2;
                it += step;
                if (_comparison(item, _bag[it]) < 0)
                {
                    first = ++it;
                    analyzingAmount -= step + 1;
                }
                else
                    analyzingAmount = step;
            }
            return first;
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Clear()
        {
            _bag.Clear();
        }

        /// <summary>
        /// Determine contains item by reference O(n)
        /// </summary>
        public bool Contains(T item)
        {
            return _bag.Contains(item);
        }

        /// <summary>
        /// O(n)
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            var copy = _bag.ToList();
            copy.Reverse();
            copy.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// O(n)
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            var copy = _bag.ToList();
            copy.Reverse();
            return copy.GetEnumerator();
        }

        /// <summary>
        /// Remove item by reference O(n)
        /// </summary>
        public bool Remove(T item)
        {
            return _bag.Remove(item);
        }

        /// <summary>
        /// Amortized O(1)
        /// </summary>
        /// <returns>Element or default(T)</returns>
        public T Take()
        {
            if (_bag.Count <= 0)
                return default(T);
            var result = _bag[_bag.Count - 1];
            _bag.RemoveAt(_bag.Count - 1);
            return result;
        }

        /// <summary>
        /// O(1)
        /// </summary>
        /// <returns>Element or default(T)</returns>
        public T Peek()
        {
            if (_bag.Count <= 0)
                return default(T);
            return _bag[_bag.Count - 1];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<T> _bag = new List<T>();
        private readonly Comparison<T> _comparison;

        public PriorityQueue(Comparison<T> comparison)
        {
            if (comparison is null)
                throw new ArgumentNullException(nameof(comparison));
            _comparison = (l,r) => -comparison(l, r);
        }

        public PriorityQueue(Comparer<T> comparer)
        {
            if (comparer is null)
                throw new ArgumentNullException(nameof(comparer));
            Comparison<T> comparison = comparer.Compare;
            _comparison = (l, r) => -comparison(l, r);
        }

        public PriorityQueue()
        {
            Comparison<T> comparison = Comparer<T>.Default.Compare;
            _comparison = (l, r) => -comparison(l, r);
        }
    }
}
