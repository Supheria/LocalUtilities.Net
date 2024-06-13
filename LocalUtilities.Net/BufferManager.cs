using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace LocalUtilities.Net;

class BufferManager(int capacity, int bufferSize)
{
    /// <summary>
    /// the total number of bytes controlled by the buffer pool  
    /// </summary>
    int Capacity { get; } = capacity;

    /// <summary>
    /// the underlying byte array maintained by the Buffer Manager,
    /// Allocates buffer space used by the buffer pool,
    /// create one big large buffer and divide that   
    /// out to each SocketAsyncEventArg objec.
    /// Allocates one large byte buffer which all I/O operations use a piece of.  
    /// This gaurds against memory fragmentation 
    /// </summary>
    byte[] Buffer { get; set; } = new byte[capacity];

    Stack<int> FreeIndexPool { get; } = new Stack<int>();

    int CurrentIndex { get; set; } = 0;

    int BufferSize { get; } = bufferSize;

    public void Relocate()
    {
        Buffer = new byte[capacity];
    }

    /// <summary>
    /// Assigns a buffer from the buffer pool to the  
    /// specified SocketAsyncEventArgs object 
    /// </summary>
    /// <param name="args"></param>
    /// <returns>true if the buffer was successfully set, otherwise false</returns>
    public bool SetBuffer(SocketAsyncEventArgs args)
    {
        if (FreeIndexPool.Count > 0)
            args.SetBuffer(Buffer, FreeIndexPool.Pop(), BufferSize);
        else
        {
            if ((Capacity - BufferSize) < CurrentIndex)
                return false;
            args.SetBuffer(Buffer, CurrentIndex, BufferSize);
            CurrentIndex += BufferSize;
        }
        return true;
    }

    /// <summary>
    /// Removes the buffer from a SocketAsyncEventArg object,
    /// This frees the buffer back to the buffer pool  
    /// </summary>
    /// <param name="args"></param>
    public void FreeBuffer(SocketAsyncEventArgs args)
    {
        FreeIndexPool.Push(args.Offset);
        args.SetBuffer(null, 0, 0);
    }
}