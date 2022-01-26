using Kashkeshet.LogicBll;
using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using Kashkeshet.Common;

namespace Kashkeshet.ConsoleUI
{
    public class ChatScreen
    {
        public Guid Id { get; set; }
        protected ChatInformationExtractor _informationExtractor;
        protected ChatUpdater _updater;

        public ChatScreen(Guid chatId, ChatInformationExtractor informationExtractor, ChatUpdater updater)
        {
            Id = chatId;
            _informationExtractor = informationExtractor;
            _updater = updater;
        }

        public void Load()
        {
            Console.Clear();
            Console.WriteLine($"Chat #{Id}");
            Console.WriteLine("---------------------------------");
            foreach(Message message in _informationExtractor.GetMessages(Id))
            {
                PrintMessage(message);
            }
        }

        public void PrintMessage(Message message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{GetUserNameById(message.SenderId)}:");
            Console.ResetColor();
            Console.WriteLine(FormatMessage(message));
        }

        public void NotifyClientJoined(Guid clientId, string name)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"User [{clientId}] aka [{name}] has joined the chat");
            Console.ResetColor();
        }

        public void NotifyClientLeft(Guid clientId)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"User [{clientId}] has left the chat");
            Console.ResetColor();
        }

        protected string FormatMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.TextMessage:
                    return Encoding.ASCII.GetString(message.MessageData);

                default:
                    return String.Empty;
            }

        }
        private string GetUserNameById(Guid userId)
        {
            IDictionary<Guid, string> clients = _informationExtractor.GetClients(Id);
            string userName = String.Empty;
            clients.TryGetValue(userId, out userName);
            return userName;
        }
    }
}
