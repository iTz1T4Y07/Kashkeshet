using Kashkeshet.Common;
using Kashkeshet.ServerCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore
{
    public class ChatsUpdater
    {
        private IList<ChatBase> _chats;

        public ChatsUpdater(IList<ChatBase> chats)
        {
            _chats = chats;
        }

        public void InitializeUpdater(ClientOrderHandler orderHandler)
        {
            orderHandler.AddMessageToChat += AddMessageToChat;
        }

        public Task AddMessageToChat(Guid chatId, Message message)
        {
            throw new NotImplementedException();

        }


    }
}
