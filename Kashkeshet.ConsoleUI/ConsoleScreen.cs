using Kashkeshet.Common;
using Kashkeshet.LogicBll;
using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
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
        private bool _isChatLoaded;

        public ConsoleScreen(ChatInformationExtractor informationExtractor, ServerCommunicator communicator, ChatUpdater updater)
        {
            _informationExtractor = informationExtractor;
            _serverCommunicator = communicator;
            _updater = updater;
            _isChatLoaded = false;
        }

        public async Task Start(CancellationToken token)
        {
            Guid chatId = await GetMainChatId();
            await InitializeUserName(token);
            InitializeData(chatId);
            await StartInputFlow(token);            
        }

        private async Task<Guid> GetMainChatId()
        {
            Guid chatId = _informationExtractor.GetMainChatId();
            int triesCounter = 0;
            while (chatId == Guid.Empty && triesCounter < 3)
            {
                
                await Task.Delay(1000);
                chatId = _informationExtractor.GetMainChatId();
                triesCounter++;
            }

            if (chatId != Guid.Empty)
            {
                return chatId;
            }

            throw new OperationCanceledException("Found 0 chats available.");
        }

        private void InitializeData(Guid chatId)
        {
            _currentChat = new ChatScreen(chatId, _informationExtractor, _updater);
            _commandHandler = new CommandHandler(_serverCommunicator, _currentChat);
            _updater.ChatMessageUpdate += ReceivedNewMessage;
            _updater.ChatClientsUpdate += ClientsListChanged;
            _currentChat.Load();
            _isChatLoaded = true;
        }

        private async Task InitializeUserName(CancellationToken token)
        {
            string userName = GetUserNameFromInput();
            JsonObject operationInfo = (JsonObject)JsonObject.Parse("{}");
            operationInfo.Add("client_name", userName);
            await _serverCommunicator.SendOperation(Operation.DeclareClientName, operationInfo, token);
        }

        private string GetUserNameFromInput()
        {
            string userName = string.Empty;
            HashSet<char> usedChars = userName.ToHashSet();
            while (userName == string.Empty || usedChars.Count < 1)
            {
                Console.WriteLine("Please enter your name");
                Console.WriteLine("Your name must contains characters other than space");
                userName = Console.ReadLine();
                usedChars = userName.ToHashSet();
                usedChars.Remove(' ');
            }
            return userName;
        }

        private async Task StartInputFlow(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            while (true)
            {
                token.ThrowIfCancellationRequested();
                string commandInput = GetCommandInput();
                await _commandHandler.HandleNewCommand(commandInput, token);
            }
        }

        private void ReceivedNewMessage(Guid chatId, Message message)
        {
            if (chatId == _currentChat.Id)
            {
                _currentChat.PrintMessage(message);
            }
        }

        private void ClientsListChanged(Guid chatId, Guid clientId)
        {
            if (chatId == _currentChat.Id)
            {
                if (_informationExtractor.GetClients(chatId).ContainsKey(clientId))
                {
                    while (!_isChatLoaded)
                    {
                        Task.Delay(500);
                    }
                    _currentChat.NotifyClientJoined(clientId, _informationExtractor.GetClients(chatId)[clientId]);
                }
                else
                {
                    _currentChat.NotifyClientLeft(clientId);
                }
            }
        }

        private string GetCommandInput()
        {
            return Console.ReadLine();
        }

    }
}
