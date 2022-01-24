using Kashkeshet.Common;
using Kashkeshet.LogicBll.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.LogicBll
{
    public class ChatInformationExtractor
    {
        private IDictionary<Guid, ChatBase> _chats;

        public ChatInformationExtractor()
        {
            _chats = new Dictionary<Guid, ChatBase>();
        }

        IList<Message> GetMessages(Guid id)
        {
            if (_chats.ContainsKey(id))
            {
                return _chats[id].GetMessages();
            }
            return new List<Message>();
        }

        IDictionary<Guid, string> GetClients(Guid id)
        {
            if (_chats.ContainsKey(id))
            {
                return _chats[id].GetClients();
            }
            return new Dictionary<Guid, string>();
        }

        public void AddNewChat(ChatBase chat)
        {
            _chats.Add(chat.Id, chat);
        }
    }
}
