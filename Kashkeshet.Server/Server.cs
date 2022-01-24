using Kashkeshet.ServerCore.Abstracts;
using Kashkeshet.ServerImplementations.Clients;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.Server
{
    public class Server
    {
        private TcpListener _listener;
        private IList<ChatBase> _chats;
        private IDictionary<ClientBase, CancellationTokenSource> _clientTokens;

        public Server(int port, IList<ChatBase> chats)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _chats = chats;
        }

        public async Task Start()
        {
            _listener.Start();
            while (_listener.Server.IsBound)
            {
                TcpClient newClientConnection = await _listener.AcceptTcpClientAsync();
                ClientBase newClient = new RegularClient(newClientConnection);
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                _clientTokens.Add(newClient, tokenSource);
                _ = newClient.ReceiveNewOrder(tokenSource.Token);
            }
        }

    }
}
