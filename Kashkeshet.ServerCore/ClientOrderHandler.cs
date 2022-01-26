using Kashkeshet.Common;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore
{
    public class ClientOrderHandler
    {
        public event Func<Guid, Message, CancellationToken, Task> NewMessageArrived; //ChatsUpdater needs to register
        public event Action<string> ClientNameChanged;

        public async Task HandleOperation(Guid senderId, Operation operation, JsonObject arguments, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            switch (operation)
            {
                case Operation.SendMessage:
                    await AddMessageToChat(senderId, arguments, token);
                    break;
                case Operation.DeclareClientName:
                    ChangeClientName(arguments, token);
                    break;
                default:
                    break;
            }
        }

        private async Task AddMessageToChat(Guid senderId, JsonObject arguments, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (!arguments.ContainsKey("chat_id") || !arguments.ContainsKey("message"))
            {
                return;
            }

            Guid chatId = Guid.Parse(arguments["chat_id"]);
            Message message = JsonSerializer.Deserialize<Message>(arguments["message"]);
            if (message.SenderId == senderId)
            {
                await NewMessageArrived?.Invoke(chatId, message, token);
            }
        }

        private void ChangeClientName(JsonObject arguments, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (!arguments.ContainsKey("client_name"))
            {
                return;
            }
            string clientName = arguments["client_name"];
            ClientNameChanged?.Invoke(clientName);
        }

    }
}
