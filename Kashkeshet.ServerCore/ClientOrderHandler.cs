using Kashkeshet.Common;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore
{
    public class ClientOrderHandler
    {
        public event Func<Guid, Message, Task> AddMessageToChat;

        public async Task HandleOperation(Operation operation, JsonObject arguments)
        {
            if (operation == Operation.SendMessage)
            {
                if (!arguments.ContainsKey("chat_id") || !arguments.ContainsKey("message"))
                {
                    return;
                }

                Guid chatId = Guid.Parse(arguments["chat_id"]);
                Message message = JsonSerializer.Deserialize<Message>(arguments["message"]);
                await AddMessageToChat?.Invoke(chatId, message);
            }
        }

    }
}
