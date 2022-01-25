using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.ConsoleUI
{
    public class CommandHandler
    {
        private ServerCommunicator _communicator;

        public Task HandleNewCommand(string command)
        {
            return Task.Run(() => { }) ;
        }
    }
}
