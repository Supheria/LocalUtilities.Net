using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalUtilities.Net;

public class AsyncClientProfile
{
    public IPAddress? IPAddress { get; set; }

    public EndPoint? RemoteEndPoint { get; set; }

    public Socket? Socket { get; set; }

    public DateTime ConnectTime { get; set; }

    public List<byte> Buffer { get; } = [];
}
