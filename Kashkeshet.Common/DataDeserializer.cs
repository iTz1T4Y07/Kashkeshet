using System;
using System.Collections.Generic;
using System.Json;
using System.Text;

namespace Kashkeshet.Common
{
    public class DataDeserializer
    {
        public byte[] Deserialize(JsonObject objectToDeserialzie)
        {
            return Encoding.UTF8.GetBytes(objectToDeserialzie.ToString());
        }
    }
}
