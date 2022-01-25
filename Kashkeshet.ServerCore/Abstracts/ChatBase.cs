using Kashkeshet.Common;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Threading;
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

        public virtual async Task UpdateAllClients(Operation operation, JsonObject arguments, CancellationToken token)
        {
            await foreach(ClientBase client in GetClients())
            {
                await client.UpdateClient(operation, arguments, token);
            }
        }

        private async IAsyncEnumerable<ClientBase> GetClients()
        {
            foreach(ClientBase client in ConnectedClients)
            {
                yield return client;
            }
        }
    }
}
