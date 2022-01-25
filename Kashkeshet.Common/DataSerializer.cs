using System;
using System.Collections.Generic;
using System.Json;
using System.Text;

namespace Kashkeshet.Common
{
    public class DataSerializer
    {
        public JsonObject Serialize(byte[] data)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            JsonObject jsonObject = (JsonObject)JsonObject.Parse(jsonString);
            return jsonObject;
        }
    }
}
