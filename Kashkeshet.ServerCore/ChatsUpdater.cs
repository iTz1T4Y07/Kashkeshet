using Kashkeshet.Common;
using Kashkeshet.ServerCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore
{
    public class ChatsUpdater
    {
        public readonly Guid MainChatId;
        private IDictionary<Guid, ChatBase> _chats;

        public ChatsUpdater(IList<ChatBase> chats)
        {
            MainChatId = chats.First().id;
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
            arguments.Add("chat_id", chatId.ToString());
            arguments.Add("message", JsonSerializer.Serialize(message, typeof(Message)));
            await _chats[chatId].UpdateAllClients(Operation.SendMessage, arguments, token);

        }

        public bool AddClientToChat(Guid chatId, ClientBase client)
        {
            if (_chats.ContainsKey(chatId))
            {
                return _chats[chatId].TryAddClient(client);
            }
            return false;
        }

        public bool RemoveClientFromChat(Guid chatId, ClientBase client)
        {
            if (_chats.ContainsKey(chatId))
            {
                return _chats[chatId].TryRemoveClient(client);
            }
            return false;
        }

        public void RemoveClientFromAllChats(ClientBase client)
        {
            foreach(Guid chatId in _chats.Keys)
            {
                RemoveClientFromChat(chatId, client);
            }
        }

        public IDictionary<Guid, string> GetChatClients(Guid chatId)
        {
            if (!_chats.ContainsKey(chatId))
            {
                return new Dictionary<Guid, string>();
            }
            return _chats[chatId].GetClients();
        }


    }
}
