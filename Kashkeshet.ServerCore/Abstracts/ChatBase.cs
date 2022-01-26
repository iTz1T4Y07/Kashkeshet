using Kashkeshet.Common;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore.Abstracts
{
    public abstract class ChatBase
    {
        public readonly Guid id = Guid.NewGuid();
        protected ISet<ClientBase> ConnectedClients;
        protected IList<Message> Messages;

        public ChatBase(ISet<ClientBase> clients)
        {
            ConnectedClients = clients;
            Messages = new List<Message>();
        }

        public bool TryAddClient(ClientBase client) => ConnectedClients.Add(client);

        public bool TryRemoveClient(ClientBase client) => ConnectedClients.Remove(client);

        public IDictionary<Guid, string> GetClients() => ConnectedClients.ToDictionary(client => client.Id, client => client.Name);
       
        public virtual async Task UpdateAllClients(Operation operation, JsonObject arguments, CancellationToken token)
        {
            await foreach(ClientBase client in GetClientsAsync())
            {
                await client.UpdateClient(operation, arguments, token);
            }
        }

        private async IAsyncEnumerable<ClientBase> GetClientsAsync()
        {
            foreach(ClientBase client in ConnectedClients)
            {
                await Task.Yield();
                yield return client;
            }
        }
    }
}
