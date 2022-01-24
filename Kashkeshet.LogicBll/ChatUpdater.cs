using Kashkeshet.Common;
using Kashkeshet.LogicBll.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.LogicBll
{
    public class ChatUpdater
    {
        public event Action<ChatBase> ChatListUpdate;
        private IDictionary<Guid, ChatBase> _chats;

        public ChatUpdater(IDictionary<Guid, ChatBase> chats)
        {
            _chats = chats;
        }

        public Task AddMessageToChat(Message message, Guid chatId)
        {
            return Task.Run(() =>
            {
                if (_chats.ContainsKey(chatId))
                {
                    _chats[chatId].AddMessage(message);
                }
            });
        }

        public async Task<bool> AddClientToChat(Guid chatId, Guid clientId, string clientName="")
        {
            if (_chats.ContainsKey(chatId))
            {
                return await _chats[chatId].AddClient(clientId, clientName);
            }
            return false;
        }

        public async Task<bool> RemoveClientFromChat(Guid chatId, Guid clientId)
        {
            if (_chats.ContainsKey(chatId))
            {
                return await _chats[chatId].RemoveClient(clientId);
            }
            return false;
        }

        public bool AddChat(ChatBase chat)
        {
            if (!_chats.ContainsKey(chat.Id))
            {
                _chats.Add(chat.Id, chat);
                ChatListUpdate?.Invoke(chat);
                return true;
            }
            return false;
        }
    }
}
