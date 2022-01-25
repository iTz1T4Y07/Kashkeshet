using System;

namespace Kashkeshet.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper bootstrapper = new Bootstrapper();
            Server server = bootstrapper.GetServer();
            server.Start().GetAwaiter().GetResult();
        }
    }
}
