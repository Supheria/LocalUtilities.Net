namespace LocalUtilities.Net.Sockets
{
    public abstract class SocketHeadContentHandler : ISocketStreamHandler
    {
        protected int HeadBufferLength { get; private set; }

        protected int ContentBufferLength { get; private set; }

        protected SocketHeadContentHandler(int headBufferLength, int contentBufferLength)
        {
            HeadBufferLength = headBufferLength;
            ContentBufferLength = contentBufferLength;
        }

        private bool CheckHeadCompleted(SocketReceiveContext context)
        {
            if (context.Buffer.Length == 0)
                return false;
            return ProcessReceiveHead(context);
        }

        private bool CheckContentCompleted(SocketReceiveContext context)
        {
            if (context.Buffer.Length == 0)
                return false;
            return ProcessReceiveContent(context);
        }

        public byte[] Receive(SocketStreamHandlerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            context.ReceiveContext.CheckQueue();

            bool headProcessCompleted = CheckHeadCompleted(context.ReceiveContext);
            if (!headProcessCompleted)
            {
                byte[] buffer = new byte[HeadBufferLength];
                long position;
                while (!headProcessCompleted)
                {
                    try
                    {
                        int length = context.Stream.Read(buffer, 0, HeadBufferLength);
                        if (length == 0)
                        {
                            return [];
                        }
                        position = context.ReceiveContext.Buffer.Position;
                        context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                        context.ReceiveContext.Buffer.Write(buffer, 0, length);
                        context.ReceiveContext.Buffer.Position = position;
                    }
                    catch
                    {
                        context.ReceiveContext.Reset();
                        return [];
                    }
                    headProcessCompleted = ProcessReceiveHead(context.ReceiveContext);
                    if (context.ReceiveContext.IsFailed)
                    {
                        context.ReceiveContext.Reset();
                        return [];
                    }
                }
            }
            bool contentProcessCompleted = CheckContentCompleted(context.ReceiveContext);
            if (!contentProcessCompleted)
            {
                byte[] buffer = new byte[ContentBufferLength];
                long position;
                while (!contentProcessCompleted)
                {
                    try
                    {
                        int length = context.Stream.Read(buffer, 0, ContentBufferLength);
                        if (length == 0)
                        {
                            return [];
                        }
                        position = context.ReceiveContext.Buffer.Position;
                        context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                        context.ReceiveContext.Buffer.Write(buffer, 0, length);
                        context.ReceiveContext.Buffer.Position = position;
                    }
                    catch
                    {
                        context.ReceiveContext.Reset();
                        return [];
                    }
                    contentProcessCompleted = ProcessReceiveContent(context.ReceiveContext);
                    if (context.ReceiveContext.IsFailed)
                    {
                        context.ReceiveContext.Reset();
                        return [];
                    }
                }
            }
            var value = context.ReceiveContext.Result;
            context.ReceiveContext.Reset();
            return value;
        }

        public async Task<byte[]> ReceiveAsync(SocketStreamHandlerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            context.ReceiveContext.CheckQueue();

            bool headProcessCompleted = CheckHeadCompleted(context.ReceiveContext);
            if (!headProcessCompleted)
            {
                if (context.ReceiveContext.IsFailed)
                {
                    context.ReceiveContext.Reset();
                    return [];
                }
                byte[] buffer = new byte[HeadBufferLength];
                long position;
                while (!headProcessCompleted)
                {
                    try
                    {
                        int length = await context.Stream.ReadAsync(buffer, 0, HeadBufferLength);
                        if (length == 0)
                        {
                            return [];
                        }
                        position = context.ReceiveContext.Buffer.Position;
                        context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                        context.ReceiveContext.Buffer.Write(buffer, 0, length);
                        context.ReceiveContext.Buffer.Position = position;
                    }
                    catch
                    {
                        context.ReceiveContext.Reset();
                        return [];
                    }
                    headProcessCompleted = ProcessReceiveHead(context.ReceiveContext);
                    if (context.ReceiveContext.IsFailed)
                    {
                        context.ReceiveContext.Reset();
                        return [];
                    }
                }
            }
            bool contentProcessCompleted = CheckContentCompleted(context.ReceiveContext);
            if (!contentProcessCompleted)
            {
                if (context.ReceiveContext.IsFailed)
                {
                    context.ReceiveContext.Reset();
                    return [];
                }
                byte[] buffer = new byte[ContentBufferLength];
                long position;
                while (!contentProcessCompleted)
                {
                    try
                    {
                        int length = await context.Stream.ReadAsync(buffer, 0, ContentBufferLength);
                        if (length == 0)
                        {
                            return [];
                        }
                        position = context.ReceiveContext.Buffer.Position;
                        context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                        context.ReceiveContext.Buffer.Write(buffer, 0, length);
                        context.ReceiveContext.Buffer.Position = position;
                    }
                    catch
                    {
                        context.ReceiveContext.Reset();
                        return [];
                    }
                    contentProcessCompleted = ProcessReceiveContent(context.ReceiveContext);
                    if (context.ReceiveContext.IsFailed)
                    {
                        context.ReceiveContext.Reset();
                        return [];
                    }
                }
            }
            var value = context.ReceiveContext.Result;
            context.ReceiveContext.Reset();
            return value;
        }

        protected abstract bool ProcessReceiveHead(SocketReceiveContext context);

        protected abstract bool ProcessReceiveContent(SocketReceiveContext context);

        public bool Send(byte[] data, SocketStreamHandlerContext context)
        {
            context.SendContext.CheckQueue();
            context.SendContext.Data = data;
            try
            {
                byte[] head = ProcessSendHead(context.SendContext);
                if (head != null)
                    context.Stream.Write(head, 0, head.Length);
                byte[] content = ProcessSendContent(context.SendContext);
                context.Stream.Write(content, 0, content.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                context.SendContext.Reset();
            }
            return true;
        }

        public async Task<bool> SendAsync(byte[] data, SocketStreamHandlerContext context)
        {
            context.SendContext.CheckQueue();
            context.SendContext.Data = data;
            try
            {
                byte[] head = ProcessSendHead(context.SendContext);
                if (head != null)
                    await context.Stream.WriteAsync(head, 0, head.Length);
                byte[] content = ProcessSendContent(context.SendContext);
                await context.Stream.WriteAsync(content, 0, content.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                context.SendContext.Reset();
            }
            return true;
        }

        protected abstract byte[] ProcessSendHead(SocketSendContext context);

        protected abstract byte[] ProcessSendContent(SocketSendContext context);
    }
}
