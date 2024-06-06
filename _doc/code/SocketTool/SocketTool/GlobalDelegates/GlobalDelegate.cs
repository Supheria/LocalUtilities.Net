using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketTool
{
   public static   class GlobalDelegate
    {
        public static  Action<Socket , string , int > AddTcpServerClientDelegate;
        public static Action< string> deleteTcpServerCommunicationDelegate;
        public static Action<string ,string> updateTcpServerReceiveStrDelegate;
    }
}
