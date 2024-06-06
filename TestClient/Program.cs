using LocalUtilities.Net.Sockets;
using System.Net;

SocketTcpClient<byte[], byte[]> client;

client = new(new SocketByteHandler());
client.SendCompleted += Receive;
Console.WriteLine("请输入IP地址：");
client.Connect(new System.Net.IPEndPoint(IPAddress.Parse(Console.ReadLine()), 5000));

byte[] data = BitConverter.GetBytes(DateTime.Now.TimeOfDay.TotalMilliseconds);
client.SendAsync(data);

async void Receive(object sender, SocketEventArgs<byte[]> e)
{
    Console.WriteLine(DateTime.Now.TimeOfDay.TotalMilliseconds - BitConverter.ToDouble(e.Data, 0));

    System.Threading.Thread.Sleep(100);
    byte[] data = BitConverter.GetBytes(DateTime.Now.TimeOfDay.TotalMilliseconds);
    await client.SendAsync(data);
}
