using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Net;

class DaemonThread
{
    Task Task { get; }

    CancellationTokenSource CancellationTokenSource { get; } = new();

    IocpServer Server { get; }

    public DaemonThread(IocpServer server)
    {
        Server = server;
        Task = new(DaemonThreadStart, CancellationTokenSource.Token);
        Task.Start();
    }

    public void DaemonThreadStart()
    {
        while (Task.Status is TaskStatus.Running)
        {
            Server.UserTokenList.CopyTo(out var tokens);
            foreach (var token in tokens)
            {
                if (Task.Status is not TaskStatus.Running)
                    break;
                try
                {
                    if ((DateTime.Now - token.SocketInfo.ActiveTime).Milliseconds > Server.TimeoutMilliseconds) //超时Socket断开
                    {
                        lock (token)
                        {
                            Server.CloseClient(token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Program.Logger.ErrorFormat("Daemon thread check timeout socket error, message: {0}", ex.Message);
                    //Program.Logger.Error(ex.StackTrace);
                }
            }

            for (int i = 0; i < 60 * 1000 / 10; i++) //每分钟检测一次
            {
                if (Task.Status is not TaskStatus.Running)
                    break;
                Thread.Sleep(10);
            }
        }
    }

    public void Close()
    {
        CancellationTokenSource.Cancel();
    }
}
