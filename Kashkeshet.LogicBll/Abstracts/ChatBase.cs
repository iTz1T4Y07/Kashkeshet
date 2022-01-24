using Kashkeshet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashkeshet.LogicBll.Abstracts
{
    public abstract class ChatBase
    {
        public readonly Guid Id;
        protected IList<IMessage> Messages;
        protected IDictionary<Guid, string> Clients;

        public virtual IList<IMessage> GetMessages() => Messages.Select(item => item).ToList();

        public virtual IDictionary<Guid, string> GetClients()
        {
            IDictionary<Guid, string> clientsCopy = new Dictionary<Guid, string>();
            foreach (Guid id in Clients.Keys)
            {
                clientsCopy.Add(id, Clients[id]);
            }
            return clientsCopy;
        }

        public virtual Task AddMessage(IMessage message)
        {
            return Task.Run(() => Messages.Add(message));
        }

        public virtual Task<bool> AddClient(Guid id, string name = "")
        {
            return Task.Run(() => Clients.TryAdd(id, name));
        }

        public virtual Task<bool> RemoveClient(Guid id)
        {
            return Task.Run(() => Clients.Remove(id));
        }
    }
}
