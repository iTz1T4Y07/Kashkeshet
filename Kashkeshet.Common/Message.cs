using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.Common
{
    [Serializable]
    public class Message
    {
        public Guid SenderId { get; }  
        public MessageType Type { get; }
        public byte[] MessageData { get; }

        [JsonConstructor]
        public Message(Guid senderId, MessageType type, byte[] messageData)
        {
            SenderId = senderId;
            Type = type;
            MessageData = messageData;
        }
    }
}
