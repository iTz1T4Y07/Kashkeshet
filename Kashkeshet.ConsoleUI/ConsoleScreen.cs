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
        private ChatUpdater _updater;
        private ChatInformationExtractor _informationExtractor;
        private ServerCommunicator _serverCommunicator;
        private CommandHandler _commandHandler;

        public ConsoleScreen(ChatInformationExtractor informationExtractor, ServerCommunicator communicator, ChatUpdater updater)
        {
            _informationExtractor = informationExtractor;
            _serverCommunicator = communicator;
            _updater = updater;
        }

        public async Task Start(CancellationToken token)
        {
            Guid chatId = _informationExtractor.GetMainChatId();
            int triesCounter = 0;
            do
            {
                await Task.Delay(1000);
                chatId = _informationExtractor.GetMainChatId();
                triesCounter++;
            }
            while (chatId == Guid.Empty && triesCounter < 3);
            if (chatId == Guid.Empty)
            {
                throw new OperationCanceledException("Found 0 chats available.");
            }
            _currentChat = new ChatScreen(chatId, _informationExtractor, _updater);
            _commandHandler = new CommandHandler(_serverCommunicator, _currentChat);
            _updater.ChatMessageUpdate += ReceivedNewMessage;
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
                await _commandHandler.HandleNewCommand(commandInput, token);
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
