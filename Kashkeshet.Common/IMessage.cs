using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet.Common
{    
    public interface IMessage
    {
        public MessageType Type { get; }

        public object GetValue();
    }
}
