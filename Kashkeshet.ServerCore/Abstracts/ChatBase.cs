using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore.Abstracts
{
    public abstract class ChatBase
    {
        public readonly Guid id = Guid.NewGuid();
        protected ISet<ClientBase> ConnectedClients;

        public ChatBase(ISet<ClientBase> clients)
        {
            ConnectedClients = clients;
        }

        public bool TryAddClient(ClientBase client) => ConnectedClients.Add(client);

        public bool TryRemoveClient(ClientBase client) => ConnectedClients.Remove(client);

        public abstract Task UpdateAllClients();
    }
}
