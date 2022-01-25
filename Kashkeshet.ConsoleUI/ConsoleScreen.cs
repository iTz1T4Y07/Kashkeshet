using Kashkeshet.LogicBll;
using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.ConsoleUI
{
    public class ConsoleScreen
    {
        private ChatInformationExtractor _informationExtractor;
        private Guid _activeChatId;
        private ServerCommunicator _serverCommunicator;
        private CommandHandler _commandHandler;

        public ConsoleScreen(ChatInformationExtractor informationExtractor, ServerCommunicator communicator, CommandHandler commandHandler)
        {
            _informationExtractor = informationExtractor;
            _serverCommunicator = communicator;
            _commandHandler = commandHandler;
        }

        public Task WaitForNewCommand()
        {            
            string input = Console.ReadLine();
            _commandHandler.HandleNewCommand(input);
        }
    }
}
