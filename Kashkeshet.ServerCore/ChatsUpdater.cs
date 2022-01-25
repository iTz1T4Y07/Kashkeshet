using Kashkeshet.Common;
using Kashkeshet.ServerCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore
{
    public class ChatsUpdater
    {
        private IDictionary<Guid, ChatBase> _chats;

        public ChatsUpdater(IList<ChatBase> chats)
        {
            _chats = new Dictionary<Guid, ChatBase>();
            foreach (ChatBase chat in chats)
            {
                _chats.Add(chat.id, chat);
            }
        }

        public void InitializeUpdater(ClientOrderHandler orderHandler)
        {
            orderHandler.AddMessageToChat += AddMessageToChat;
        }

        public async Task AddMessageToChat(Guid chatId, Message message, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (!_chats.ContainsKey(chatId))
            {
                return;
            }
            IEnumerable<KeyValuePair<string, JsonValue>> items = new List<KeyValuePair<string, JsonValue>>();
            JsonObject arguments = new JsonObject(items);
            arguments.Add("chat_id", chatId);
            arguments.Add("message", JsonSerializer.Serialize(message, typeof(Message)));
            await _chats[chatId].UpdateAllClients(Operation.SendMessage, arguments, token);

        }


    }
}
