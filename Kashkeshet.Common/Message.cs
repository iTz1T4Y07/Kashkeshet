using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.Common
{
    public class Message
    {
        public MessageType Type { get; }
        public byte[] messageData { get; }
    }
}
