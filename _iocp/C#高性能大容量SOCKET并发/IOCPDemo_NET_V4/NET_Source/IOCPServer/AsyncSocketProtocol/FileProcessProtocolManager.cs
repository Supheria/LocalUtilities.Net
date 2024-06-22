using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net;

public class FileProcessProtocolManager<T> : IEnumerable<T> where T : FileStreamProtocol
{
    List<T> List { get; } = [];

    public int Count()
    {
        return List.Count;
    }

    public T ElementAt(int index)
    {
        return List.ElementAt(index);
    }

    public void Add(T value)
    {
        List.Add(value);
    }

    public void Remove(T value)
    {
        List.Remove(value);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return List.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return List.GetEnumerator();
    }
}
