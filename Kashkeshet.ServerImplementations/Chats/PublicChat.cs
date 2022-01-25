using Kashkeshet.ServerCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.ServerImplementations.Chats
{
    public class PublicChat : ChatBase
    {
        public PublicChat(ISet<ClientBase> clients) : base(clients)
        {
        }

    }
}
