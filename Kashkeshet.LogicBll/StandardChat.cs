using Kashkeshet.Common;
using Kashkeshet.LogicBll.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.LogicBll
{
    public class StandardChat : ChatBase
    {
        public StandardChat(Guid id, IList<Message> messages, IDictionary<Guid, string> clients) : base(id, messages, clients)
        {
        }
    }
}
