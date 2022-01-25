using Kashkeshet.Common;
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
        private ChatScreen _currentChat;
        private ChatInformationExtractor _informationExtractor;
        private ServerCommunicator _serverCommunicator;
        private CommandHandler _commandHandler;

        public ConsoleScreen(ChatInformationExtractor informationExtractor, ServerCommunicator communicator, CommandHandler commandHandler, ChatUpdater updater)
        {
            _informationExtractor = informationExtractor;
            _serverCommunicator = communicator;
            _commandHandler = commandHandler;
            Guid chatId = informationExtractor.GetMainChatId();
            if (chatId == Guid.Empty)
            {
                throw new OperationCanceledException("Found 0 chats available.");
            }
            _currentChat = new ChatScreen(chatId, informationExtractor, updater);
        }

        public Task WaitForNewCommand()
        {
            throw new NotImplementedException();
        }

        public Task ReceivedNewMessage(Guid chatId, Message message)
        {
            return Task.Run(() =>
            {
                if (chatId == _currentChat.Id)
                {
                    _currentChat.PrintMessage(message);
                }
            });
        }

    }
}
