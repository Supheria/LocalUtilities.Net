namespace LocalUtilities.Net.Sockets
{
    public class SocketHandler32 : SocketHeadContentHandler
    {
        public SocketHandler32() : base(4, 4096) { }

        protected override bool ProcessReceiveHead(SocketReceiveContext context)
        {
            if (context.Buffer.Length < 4)
                return false;
            byte[] data = new byte[4];
            context.Buffer.Read(data, 0, 4);
            context.DataBag.DataLength = BitConverter.ToInt32(data, 0);
            return true;
        }

        protected override bool ProcessReceiveContent(SocketReceiveContext context)
        {
            int length = context.DataBag.DataLength;
            if (context.Buffer.Length < length)
                return false;
            byte[] data = new byte[length];
            context.Buffer.Read(data, 0, length);
            context.Result = data;
            return true;
        }

        protected override byte[] ProcessSendHead(SocketSendContext context)
        {
            return BitConverter.GetBytes(context.Data.Length);
        }

        protected override byte[] ProcessSendContent(SocketSendContext context)
        {
            return context.Data;
        }
    }
}
