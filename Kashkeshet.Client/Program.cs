using Kashkeshet.ConsoleUI;
using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Json;
using System.Net;
using System.Text;
using System.Threading;

namespace Kashkeshet.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper bootstrapper = new Bootstrapper();

            ServerCommunicator serverCommunicator = bootstrapper.GetServerCommunicator();
            ConsoleScreen consoleScreen = bootstrapper.GetConsoleScreen();
            if (serverCommunicator.TryConnect(IPAddress.Loopback, 9090).GetAwaiter().GetResult())
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                serverCommunicator.WaitForIncomingUpdates(tokenSource.Token);
                consoleScreen.Start(tokenSource.Token).GetAwaiter().GetResult();
            }

        }
    }
}
