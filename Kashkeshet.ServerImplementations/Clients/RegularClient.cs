using Kashkeshet.Common;
using Kashkeshet.ServerCore;
using Kashkeshet.ServerCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ServerImplementations.Clients
{
    public class RegularClient : ClientBase
    {
        public RegularClient(TcpClient client, DataSerializer serializer, DataDeserializer deserializer, ClientOrderHandler orderHandler) : base(client, serializer, deserializer, orderHandler)
        {
        }
    }
}
