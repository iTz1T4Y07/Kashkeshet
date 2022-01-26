using Kashkeshet.Common;
using Kashkeshet.NetworkBll;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ConsoleUI
{
    public class CommandHandler
    {
        private ServerCommunicator _communicator;
        private ChatScreen _currentChat;

        public CommandHandler(ServerCommunicator communicator, ChatScreen chat)
        {
            _communicator = communicator;
            _currentChat = chat;
        }

        public async Task HandleNewCommand(string command, CancellationToken token)
        {
            Message newMessage = new Message(_communicator.UserId, MessageType.TextMessage, Encoding.ASCII.GetBytes(command));
            string messageJsonString = JsonSerializer.Serialize(newMessage);
            JsonObject arguments = (JsonObject)JsonObject.Parse("{}");
            arguments.Add("chat_id", _currentChat.Id.ToString());
            arguments.Add("message", messageJsonString);

            await _communicator.SendOperation(Operation.SendMessage, arguments, token);
        }
    }
}
