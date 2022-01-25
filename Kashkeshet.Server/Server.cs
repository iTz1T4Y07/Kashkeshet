using Kashkeshet.Common;
using Kashkeshet.ServerCore;
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
        private ChatsUpdater _chatsUpdater;
        private IDictionary<ClientBase, CancellationTokenSource> _clientTokens;

        public Server(int port, IList<ChatBase> chats)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _chatsUpdater = new ChatsUpdater(chats);
        }

        public async Task Start()
        {
            _listener.Start();
            DataSerializer serializer = new DataSerializer();
            DataDeserializer deserializer = new DataDeserializer();
            ClientOrderHandler clientOrderHandler = new ClientOrderHandler();
            _chatsUpdater.InitializeUpdater(clientOrderHandler);
            while (_listener.Server.IsBound)
            {
                TcpClient newClientConnection = await _listener.AcceptTcpClientAsync();
                ClientBase newClient = new RegularClient(newClientConnection, serializer, deserializer, clientOrderHandler);
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                _clientTokens.Add(newClient, tokenSource);
                _chatsUpdater.AddClientToChat(_chatsUpdater.MainChatId, newClient);
                _ = newClient.ReceiveNewOrder(tokenSource.Token);
            }
        }

    }
}
