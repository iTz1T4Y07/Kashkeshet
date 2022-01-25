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
                case Operation.AddNewChat:
                    await AddNewChat(arguments);
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

        private Task AddNewChat(JsonObject arguments)
        {
            return Task.Run(() =>
            {
                if (!arguments.ContainsKey("chat_id") || !arguments.ContainsKey("clients"))
                {
                    return;
                }

                Guid chatId = Guid.Parse(arguments["chat_id"]);
                IList<Message> messages = new List<Message>();
                IDictionary<Guid, string> clients = new Dictionary<Guid, string>();
                JsonObject clientsJson = (JsonObject)JsonObject.Parse(arguments["clients"]);
                foreach (string jsonKey in clientsJson.Keys)
                {
                    clients.Add(Guid.Parse(jsonKey), clientsJson[jsonKey]);
                }

                _updater.AddChat(new Chat(chatId, messages, clients));
            });
        }
    }
}
