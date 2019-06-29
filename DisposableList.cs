using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Global.BusinessCommon.Helpers.ExceptionHelpers;

namespace Global.BusinessCommon.Helpers.Containers
{
    public interface IDisposableList<DisposableItem> : IList<DisposableItem>, IDisposable where DisposableItem : class, IDisposable
    { }

    public class DisposableList<DisposableItem> : IDisposableList<DisposableItem>
        where DisposableItem : class, IDisposable
    {
        private readonly List<DisposableItem> _items = new List<DisposableItem>();

        public void Dispose()
        {
            _items.ForEach(i => ExceptionHelper.ExceptionCatcher(i.Dispose, where: MethodBase.GetCurrentMethod().ToString()));
            _items.Clear();
        }

        public IEnumerator<DisposableItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(DisposableItem item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(DisposableItem item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(DisposableItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(DisposableItem item)
        {
            return _items.Remove(item);
        }

        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public int IndexOf(DisposableItem item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, DisposableItem item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public DisposableItem this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
    }
}
