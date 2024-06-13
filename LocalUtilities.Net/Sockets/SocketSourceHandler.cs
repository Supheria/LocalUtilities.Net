namespace LocalUtilities.Net.Sockets
{
    public class SocketSourceHandler : ISocketDgramHandler
    {
        public byte[] ConvertReceiveData(byte[] data, int length)
        {
            if (data.Length != length)
                return data.Take(length).ToArray();
            return data;
        }

        public byte[] ConvertSendData(byte[] obj)
        {
            return obj;
        }
    }
}
