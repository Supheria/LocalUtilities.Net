using System.Net;
using WarringStates.UI;

namespace IOCPServer;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.Run(new ServerForm());
    }
}