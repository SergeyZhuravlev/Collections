
using System.Threading;

namespace Global.BusinessCommon.Helpers.Containers
{
    public interface IBlockingQueue<T>: IQueue<T>
    {
        T Take(CancellationToken token);
    }
}