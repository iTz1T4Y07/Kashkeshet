using Kashkeshet.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.LogicBll
{
    public class ChatUpdater
    {
        public event Action<Chat> ChatListUpdate;
        public event Action<Guid, Message> ChatMessageUpdate;
        public event Action<Guid, Guid > ChatClientsUpdate;
        public Guid UserId { get; set; }

        private IDictionary<Guid, Chat> _chats;

        public ChatUpdater(Guid userId, IDictionary<Guid, Chat> chats)
        {
            UserId = userId;
            _chats = chats;
        }

        public Task AddMessageToChat(Message message, Guid chatId)
        {
            return Task.Run(() =>
            {
                if (_chats.ContainsKey(chatId))
                {
                    _chats[chatId].AddMessage(message);
                    if (UserId != message.SenderId)
                    {
                        ChatMessageUpdate?.Invoke(chatId, message);
                    }
                }
            });
        }

        public async Task<bool> AddClientToChat(Guid chatId, Guid clientId, string clientName="")
        {
            if (_chats.ContainsKey(chatId))
            {
                if(await _chats[chatId].AddClient(clientId, clientName))
                {
                    ChatClientsUpdate?.Invoke(chatId, clientId);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> RemoveClientFromChat(Guid chatId, Guid clientId)
        {
            if (_chats.ContainsKey(chatId))
            {
                if(await _chats[chatId].RemoveClient(clientId))
                {
                    ChatClientsUpdate.Invoke(chatId, clientId);
                    return true;
                }
            }
            return false;
        }

        public bool AddChat(Chat chat)
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
