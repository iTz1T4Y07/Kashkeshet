using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.Common
{
    public interface IMessage<T>
    {
        public MessageType Type { get; }

        public T GetValue();
    }
}
