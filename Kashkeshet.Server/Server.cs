﻿using Kashkeshet.Common;
using Kashkeshet.ServerCore;
using Kashkeshet.ServerCore.Abstracts;
using Kashkeshet.ServerImplementations.Clients;
using System;
using System.Collections.Generic;
using System.Json;
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
            _clientTokens = new Dictionary<ClientBase, CancellationTokenSource>();
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

                await Task.Run(() => RegisterNewClient(newClient, tokenSource));

                if (!await SendBasicInformationToClient(newClient, tokenSource.Token))
                {
                    newClientConnection.Close();
                }

                _ = Task.Run(() => newClient.ReceiveNewOrder(tokenSource.Token));
            }
        }

        private void RegisterNewClient(ClientBase client, CancellationTokenSource source)
        {
            client.UpdateClientDisconnect += HandleClientDisconnection;
            _clientTokens.Add(client, source);
            _chatsUpdater.AddClientToChat(_chatsUpdater.MainChatId, client);
        }

        private async Task<bool> SendBasicInformationToClient(ClientBase client, CancellationToken token)
        {
            Task<bool> sendClientId = client.UpdateClient(Operation.ClientIdExchange, GetClientIdArguments(client), token);
            Task<bool> sendClientMainChat = client.UpdateClient(Operation.AddNewChat, GetMainChatArguments(), token);
            bool[] isBasicInformationSentSucessfully = await Task.WhenAll(sendClientId, sendClientMainChat);
            foreach (bool status in isBasicInformationSentSucessfully)
            {
                if (!status)
                {
                    return false;
                }
            }
            return true;
        }

        private void HandleClientDisconnection(object Sender, EventArgs args)
        {
            _chatsUpdater.RemoveClientFromAllChats((ClientBase)Sender);
        }

        private JsonObject GetClientIdArguments(ClientBase client)
        {
            JsonObject arguments = (JsonObject)JsonObject.Parse("{}");
            arguments.Add("client_id", client.Id.ToString());
            return arguments;
        }

        private JsonObject GetMainChatArguments()
        {
            JsonObject arguments = (JsonObject)JsonObject.Parse("{}");
            arguments.Add("chat_id", _chatsUpdater.MainChatId.ToString());
            JsonObject clientsJson = (JsonObject)JsonObject.Parse("{}");
            IDictionary<Guid, string> clients = _chatsUpdater.GetChatClients(_chatsUpdater.MainChatId);
            foreach (Guid id in clients.Keys)
            {
                clientsJson.Add(id.ToString(), clients[id]);
            }
            arguments.Add("clients", clientsJson);
            return arguments;
        }

    }
}
