using Kashkeshet.Common;
using Kashkeshet.LogicBll;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kashkeshet.NetworkBll
{
    public class OperationHandler
    {
        private ChatUpdater _updater;

        public OperationHandler(ChatUpdater updater)
        {
            _updater = updater;
        }

        public async Task HandleNewOperation(Operation operation, JsonObject arguments)
        {
            switch (operation)
            {
                case Operation.SendMessage:
                    await SendMessage(arguments);
                    break;
                default:
                    break;
            }
        }

        public async Task SendMessage(JsonObject arguments)
        {
            if (!arguments.ContainsKey("chat_id") || !arguments.ContainsKey("message"))
            {
                return;
            }            
            Message message = JsonSerializer.Deserialize<Message>(arguments["message"]);            
            await _updater.AddMessageToChat(message, arguments["chat_id"]);

        }
    }
}
