using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.BusinessCommon.Helpers.Constraints;

namespace Global.BusinessCommon.Helpers.Containers
{
    public class LifoCache<Key, Value> : IDictionary<Key, Value>
    {
        public int Capacity { get; }

        public LifoCache(int capacity)
        {
            if(capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity) + " lower then 1");
            this.Capacity = capacity;
        }

        public Value this[Key key]
        {
            get
            {
                lock (SyncRoot)
                    return _dictionary[key].Value.Value;
            }
            set
            {
                lock (SyncRoot)
                {
                    InsertOrUpdateInternal(key, value);
                }
            }
        }

        private bool InsertOrUpdateInternal(Key key, Value value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key].Value = new KeyValuePair<Key, Value>(key, value);
                return false;
            }
            if (_queue.Count >= Capacity)
            {
                var last = _queue.Last();
                _dictionary.Remove(last.Key);
                _queue.RemoveLast();
            }
            _dictionary[key] = _queue.AddFirst(new KeyValuePair<Key, Value>(key, value));
            return true;
        }

        public ICollection<Key> Keys
        {
            get
            {
                lock (SyncRoot) return _queue.Select(item => item.Key).ToList().AsReadOnly();
            }
        }

        public ICollection<Value> Values
        {
            get
            {
                lock (SyncRoot) return _queue.Select(item => item.Value).ToList().AsReadOnly();
            }
        }

        public int Count
        {
            get
            {
                lock (SyncRoot) return _queue.Count;
            }
        }

        public bool IsReadOnly => false;

        public void Add(Key key, Value value)
        {
            lock (SyncRoot)
            {
                if (_dictionary.ContainsKey(key))
                    throw new ArgumentException(nameof(key) + " already exist");
                InsertOrUpdateInternal(key, value);
            }
        }

        public void Add(KeyValuePair<Key, Value> item)
        {
            lock (SyncRoot)
            {
                if (_dictionary.ContainsKey(item.Key))
                    throw new ArgumentException(nameof(item.Key) + " already exist");
                InsertOrUpdateInternal(item.Key, item.Value);
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                _dictionary.Clear();
                _queue.Clear();
            }
        }

        public bool Contains(KeyValuePair<Key, Value> item)
        {
            lock (SyncRoot) return !(_queue.Find(item) is null);
        }

        public bool ContainsKey(Key key)
        {
            lock (SyncRoot) return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex)
        {
            IList<KeyValuePair<Key, Value>> result;
            lock (SyncRoot)
            {
                result = 
                    _dictionary
                    .Select(pair => pair.Value.Value)
                    .ToList();
            }
            result.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
        {
            lock (SyncRoot)
            {
                return 
                    _dictionary
                    .Select(pair => pair.Value.Value)
                    .ToList()
                    .GetEnumerator();
            }
        }

        public bool Remove(Key key)
        {
            lock (SyncRoot)
            {
                if (!_dictionary.ContainsKey(key))
                    return false;
                _queue.Remove(_dictionary[key]);
                _dictionary.Remove(key);
                return true;
            }
        }

        public bool Remove(KeyValuePair<Key, Value> item)
        {
            lock (SyncRoot)
            {
                var finded = _queue.Find(item);
                if (finded is null)
                    return false;
                _queue.Remove(finded);
                _dictionary.Remove(item.Key);
                return true;
            }
        }

        public bool TryGetValue(Key key, out Value value)
        {
            lock (SyncRoot)
            {
                var result = _dictionary.TryGetValue(key, out var valueNode);
                value = result ? valueNode.Value.Value : default;
                return result;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly object SyncRoot = new object();
        private LinkedList<KeyValuePair<Key, Value>> _queue = new LinkedList<KeyValuePair<Key, Value>>();
        private Dictionary<Key, LinkedListNode<KeyValuePair<Key, Value>>> _dictionary = new Dictionary<Key, LinkedListNode<KeyValuePair<Key, Value>>>();
    }
}
