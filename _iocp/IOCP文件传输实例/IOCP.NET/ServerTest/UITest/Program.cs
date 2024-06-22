using System;
using System.Windows.Forms;
using WarringStates.UI;

namespace ServerTest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new ServerForm());
        }
    }
}
