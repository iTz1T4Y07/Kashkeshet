using Kashkeshet.ServerCore.Abstracts;
using Kashkeshet.ServerImplementations.Chats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.Server
{
    public class Bootstrapper
    {
        public Server GetServer()
        {
            IList<ChatBase> chats = new List<ChatBase>();
            chats.Add(new PublicChat(new HashSet<ClientBase>()));
            Server server = new Server(9090, chats);
            return server;
        }
    }
}
