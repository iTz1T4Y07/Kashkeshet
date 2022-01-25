using Kashkeshet.ServerCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.ServerCore
{
    public class ChatsUpdater
    {
        private IList<ChatBase> _chats;

        public ChatsUpdater(IList<ChatBase> chats)
        {
            _chats = chats;
        }
    }
}
