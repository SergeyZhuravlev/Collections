using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Global.BusinessCommon.Helpers.Containers;

namespace Global.BusinessCommon.Helpers.Multithreading
{
    public class BlockingPriorityQueue<T>: IBlockingQueue<T>, ICollection<T>
    {
        public BlockingPriorityQueue(Comparison<T> comparison)
        {
            _bag = new PriorityQueue<T>(comparison);
        }

        public BlockingPriorityQueue(Comparer<T> comparer)
        {
            _bag = new PriorityQueue<T>(comparer);
        }

        public BlockingPriorityQueue()
        {
            _bag = new PriorityQueue<T>();
        }

        public void Add(T item)
        {
            lock (_syncRoot)
            {
                _bag.Add(item);
                Monitor.PulseAll(_syncRoot);
            }
        }

        public void AddMany(IEnumerable<T> items)
        {
            lock (_syncRoot)
            {
                _bag.AddRange(items);
                Monitor.PulseAll(_syncRoot);
            }
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                _bag.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_syncRoot)
            {
                return _bag.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_syncRoot)
            {
                _bag.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (_syncRoot)
            {
                return _bag.Remove(item);
            }
        }

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _bag.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public T Take()
        {
            return Take(CancellationToken.None);
        }

        public T Peek()
        {
            lock (_syncRoot)
            {
                return _bag.Peek();
            }
        }

        private void SynchronizedWakeUp()
        {
            lock (_syncRoot)
            {
                Monitor.PulseAll(_syncRoot);
            }
        }

        public T Take(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            lock (_syncRoot)
            {
                token.ThrowIfCancellationRequested();
                if (_bag.Count <= 0)
                {
                    using(token.Register(SynchronizedWakeUp))
                    do
                    {
                        token.ThrowIfCancellationRequested();
                        Monitor.Wait(_syncRoot, TimeSpan.FromSeconds(30));//timeout for avoid deadlock when cancellation token was signaled not in Monitor.Wait state.
                    } while (_bag.Count <= 0);//-V3022
					//-V3022
                }
                token.ThrowIfCancellationRequested();
                return _bag.Take();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _bag.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly PriorityQueue<T> _bag;
        private readonly object _syncRoot = new object();
    }
}
