using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LocalUtilities.Net;

public class SocketEventPool(int capacity)
{
    Stack<SocketAsyncEventArgs> Pool { get; } = new(capacity);

    public int Count => Pool.Count;

    public void Push(SocketAsyncEventArgs item)
    {
        ArgumentNullException.ThrowIfNull(item);
        lock (Pool)
        {
            Pool.Push(item);
        }
    }

    /// <summary>
    /// Removes a SocketAsyncEventArgs instance from the pool,
    ///  and returns the object removed from the pool
    /// </summary>
    /// <returns></returns>
    public SocketAsyncEventArgs Pop()
    {
        lock (Pool)
        {
            return Pool.Pop();
        }
    }

    public void Clear()
    {
        Pool.Clear();
    }
}
