using Kashkeshet.Common;
using Kashkeshet.LogicBll;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.NetworkBll
{
    public class ServerCommunicator
    {
        public Guid UserId { get; private set; }

        private OperationHandler _handler;
        private TcpClient _client;
        private DataSerializer _serializer;
        private DataDeserializer _deserializer;

        public ServerCommunicator(OperationHandler operationHandler, DataSerializer serializer, DataDeserializer deserializer)
        {
            UserId = Guid.Empty;
            _client = new TcpClient();
            _handler = operationHandler;
            _serializer = serializer;
            _deserializer = deserializer;
        }

        public Task<bool> TryConnect(IPAddress ip, int port)
        {
            return Task.Run(() =>
            {
                _client.Connect(ip, port);
                return _client.Connected;
            });
        }

        public async Task<bool> SendOperation(Operation requiredOperation, JsonObject operationArguments, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            NetworkStream stream = _client.GetStream();
            if (stream.CanWrite)
            {                
                await stream.WriteAsync(FormatNetworkMessage(requiredOperation, operationArguments));
                return true;
            }
            return false;
        }

        public async Task WaitForIncomingUpdates(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            NetworkStream stream = _client.GetStream();
            while (stream.CanRead)
            {
                token.ThrowIfCancellationRequested();
                byte[] receivedData = await ReadNewMessage(stream, token);
                token.ThrowIfCancellationRequested();
                Operation operation = Enum.Parse<Operation>(receivedData[0].ToString());
                JsonObject argumentsReceived = _serializer.Serialize(receivedData.TakeLast(receivedData.Length - 1).ToArray());
                await _handler.HandleNewOperation(operation, argumentsReceived);
            }
        }

        private byte[] FormatNetworkMessage(Operation requiredOperation, JsonObject arguments)
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

        private async Task<byte[]> ReadNewMessage(NetworkStream stream, CancellationToken token)
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
    }
}
