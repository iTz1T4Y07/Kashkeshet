using Kashkeshet.Common;
using Kashkeshet.LogicBll;
using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
            updater.ChatMessageUpdate += ReceivedNewMessage;
        }

        public async Task Start(CancellationToken token)
        {
            _currentChat.Load();
            await StartInputFlow(token);            
        }
        public async Task StartInputFlow(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            while (true)
            {
                token.ThrowIfCancellationRequested();
                string commandInput = GetCommandInput();
                await _commandHandler.HandleNewCommand(commandInput);
            }
        }

        public void ReceivedNewMessage(Guid chatId, Message message)
        {
            if (chatId == _currentChat.Id)
            {
                _currentChat.PrintMessage(message);
            }
        }

        private string GetCommandInput()
        {
            return Console.ReadLine();
        }

    }
}
