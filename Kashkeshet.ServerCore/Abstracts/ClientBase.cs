using Kashkeshet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore.Abstracts
{
    public abstract class ClientBase
    {
        public event EventHandler UpdateClientDisconnect;
        public readonly Guid Id = Guid.NewGuid();
        protected TcpClient Client;
        public string Name { get; protected set; }

        private DataSerializer _serializer;
        private DataDeserializer _deserializer;
        private ClientOrderHandler _orderHandler;

        public ClientBase(TcpClient client, DataSerializer serializer, DataDeserializer deserializer, ClientOrderHandler orderHandler)
        {
            Client = client;
            Name = string.Empty;
            _serializer = serializer;
            _deserializer = deserializer;
            _orderHandler = orderHandler;
            _orderHandler.ClientNameChanged += (newName => Name = newName);
        }

        public async Task ListenForNewOrders(CancellationToken token) // Receiving new orders from network
        {
            token.ThrowIfCancellationRequested();
            NetworkStream stream = Client.GetStream();
            while (Client.Connected && stream.CanRead)
            {
                await ReceiveOneOrder(token);
            }            
        }

        public async Task<bool> ReceiveOneOrder(CancellationToken token)
        {
            byte[] receivedData;
            NetworkStream stream = Client.GetStream();
            token.ThrowIfCancellationRequested();
            try
            {
                receivedData = await ReadNewMessage(stream, token);
            }
            catch (IOException e)
            {
                Client.Close();
                UpdateClientDisconnect?.Invoke(this, EventArgs.Empty);
                return false;
            }
            token.ThrowIfCancellationRequested();
            await HandleNewOrder(receivedData, token);
            return true;
        }

        public virtual async Task<bool> UpdateClient(Operation operation, JsonObject operationArguments, CancellationToken token) // Sending updates to remote client via network
        {
            token.ThrowIfCancellationRequested();
            NetworkStream stream = Client.GetStream();
            if (Client.Connected && stream.CanWrite)
            {
                await stream.WriteAsync(FormatNetworkMessage(operation, operationArguments));
                return true;
            }
            return false;
        }

        protected virtual async Task HandleNewOrder(byte[] data, CancellationToken token) // Handling new order
        {
            Operation operationReceived = Enum.Parse<Operation>(data[0].ToString());
            JsonObject argumentsReceived = _serializer.Serialize(data.TakeLast(data.Length - 1).ToArray());
            await _orderHandler.HandleOperation(operationReceived, argumentsReceived, token);
        }

        protected async Task<byte[]> ReadNewMessage(NetworkStream stream, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            byte messageType = (byte)stream.ReadByte();
            byte[] messageLengthBuffer = new byte[sizeof(int)];
            int receivedData = await stream.ReadAsync(messageLengthBuffer, 0, messageLengthBuffer.Length);
            int messageLength = BitConverter.ToInt32(messageLengthBuffer, 0);
            byte[] messageBuffer = new byte[messageLength];
            receivedData = await stream.ReadAsync(messageBuffer, 0, messageBuffer.Length);
            byte[] receivedMessage = new byte[messageLength + 1];
            receivedMessage[0] = messageType;
            messageBuffer.CopyTo(receivedMessage, 1);
            return receivedMessage;
        }

        protected byte[] FormatNetworkMessage(Operation requiredOperation, JsonObject arguments)
        {
            byte[] message = _deserializer.Deserialize(arguments);
            byte[] length = BitConverter.GetBytes(message.Length);
            byte[] buffer = new byte[sizeof(int) + message.Length + 1];
            buffer[0] = ((byte)requiredOperation);
            for (int i = 0; i < sizeof(int); i++)
            {
                buffer[i + 1] = length[i];
            }

            for (int i = 0; i < message.Length; i++)
            {
                buffer[i + 1 + sizeof(int)] = message[i];
            }
            return buffer;
        }
    }
}
