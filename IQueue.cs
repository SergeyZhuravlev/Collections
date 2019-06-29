using System;
using System.Collections.Generic;
using System.Text;

namespace Global.BusinessCommon.Helpers.Containers
{
    public interface IQueue<T>
    {
        void Add(T item);
        T Take();
        T Peek();
    }
}
