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
        public event Action<Guid> UpdateClientId;

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
                case Operation.ClientIdExchange:
                    UpdateId(arguments);
                    break;
                default:
                    break;
            }
        }

        private async Task SendMessage(JsonObject arguments)
        {
            if (!arguments.ContainsKey("chat_id") || !arguments.ContainsKey("message"))
            {
                return;
            }            
            Message message = JsonSerializer.Deserialize<Message>(arguments["message"]);            
            await _updater.AddMessageToChat(message, Guid.Parse(arguments["chat_id"]));

        }

        private void UpdateId(JsonObject arguments)
        {
            if (!arguments.ContainsKey("client_id"))
            {
                return;
            }
            UpdateClientId?.Invoke(Guid.Parse(arguments["client_id"]));
        }
    }
}
