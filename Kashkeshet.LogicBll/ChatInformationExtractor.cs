using Kashkeshet.Common;
using Kashkeshet.LogicBll.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kashkeshet.LogicBll
{
    public class ChatInformationExtractor
    {
        private IDictionary<Guid, Chat> _chats;

        public ChatInformationExtractor()
        {
            _chats = new Dictionary<Guid, Chat>();
        }

        public IList<Message> GetMessages(Guid id)
        {
            if (_chats.ContainsKey(id))
            {
                return _chats[id].GetMessages();
            }
            return new List<Message>();
        }

        public IDictionary<Guid, string> GetClients(Guid id)
        {
            if (_chats.ContainsKey(id))
            {
                return _chats[id].GetClients();
            }
            return new Dictionary<Guid, string>();
        }

        public void AddNewChat(Chat chat)
        {
            _chats.Add(chat.Id, chat);
        }

        public Guid GetMainChatId()
        {
            if(_chats.Count > 0)
            {
                return _chats.First().Key;
            }
            return Guid.Empty;
        }
    }
}
