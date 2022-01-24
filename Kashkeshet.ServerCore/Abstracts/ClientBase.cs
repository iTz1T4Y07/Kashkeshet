using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ServerCore.Abstracts
{
    public abstract class ClientBase
    {

        public readonly Guid Id = Guid.NewGuid();
        protected TcpClient Client;
        protected string Name;

        public ClientBase(TcpClient client)
        {
            Client = client;
            Name = string.Empty;
        }

        public async Task ReceiveNewOrder(CancellationToken token) // Receiving new orders from network
        {
            token.ThrowIfCancellationRequested();
            NetworkStream stream = Client.GetStream();
            while (stream.CanRead)
            {
                token.ThrowIfCancellationRequested();
                byte[] receivedData = await ReadNewMessage(stream, token);
                token.ThrowIfCancellationRequested();
                await HandleNewOrder(receivedData, token);
            }
        }

        public abstract Task<bool> UpdateClient(); // Sending updates to remote client via network
        protected abstract Task HandleNewOrder(byte[] data, CancellationToken token); // Handling new order

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
    }
}
