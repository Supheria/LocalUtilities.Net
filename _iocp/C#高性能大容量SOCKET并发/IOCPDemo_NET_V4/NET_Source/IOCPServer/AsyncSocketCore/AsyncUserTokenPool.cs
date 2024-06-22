using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net;

public class AsyncUserTokenPool(int capacity)
{
    Stack<AsyncUserToken> Pool { get; } = new(capacity);

    public void Push(AsyncUserToken item)
    {
        //if (item == null)
        //{
        //    throw new ArgumentException("Items added to a AsyncSocketUserToken cannot be null");
        //}
        lock (Pool)
        {
            Pool.Push(item);
        }
    }

    public AsyncUserToken Pop()
    {
        lock (Pool)
        {
            return Pool.Pop();
        }
    }

    public int Count => Pool.Count;
}
