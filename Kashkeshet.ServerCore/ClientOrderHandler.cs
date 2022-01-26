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

        public async Task HandleOperation(Operation operation, JsonObject arguments, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            switch (operation)
            {
                case Operation.SendMessage:
                    await AddMessageToChat(arguments, token);
                    break;
            }
        }

        private async Task AddMessageToChat(JsonObject arguments, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (!arguments.ContainsKey("chat_id") || !arguments.ContainsKey("message"))
            {
                return;
            }

            Guid chatId = Guid.Parse(arguments["chat_id"]);
            Message message = JsonSerializer.Deserialize<Message>(arguments["message"]);
            await NewMessageArrived?.Invoke(chatId, message, token);
        }

    }
}
