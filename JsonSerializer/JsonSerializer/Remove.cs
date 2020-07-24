using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSerializer
{
    public static partial class Json
    {
        public static string RemoveKeyValuePair(string jsonObject, string key)
        {
            var dict = DeserializeObject(jsonObject);

            foreach (var k in dict.Keys)
            {
                if (DeserializeString(k) == DeserializeString(key))
                {
                    dict.Remove(k);
                }
            }

            return SerializeObject(dict);

            
        }

    }
}
