using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JsonSerializer
{
    public static partial class Json
    {

        public static List<string[]> FindKeyValuePair(string jsonData, string key, string value)
        {
            return FindKeyValuePair(jsonData, key, value, ValueType.Unspecified, false, false, -1);
        }

        public static List<string[]> FindKeyValuePair(string jsonData, string key, ValueType valueType)
        {
            return FindKeyValuePair(jsonData, key, null, valueType, false, false, -1);
        }

        public static List<string[]> FindKeyValuePair(string jsonData, string key, string value, ValueType valueType)
        {
            return FindKeyValuePair(jsonData, key, value, valueType, false, false, -1);
        }

        public static List<string[]> FindKeyValuePair(string jsonData, string key, string value, ValueType valueType, bool returnFirstInstance)
        {
            return FindKeyValuePair(jsonData, key, value, valueType, false, returnFirstInstance, -1);
        }

        public static List<string[]> FindKeyValuePair(string jsonData, string key, string value, ValueType valueType, bool caseSensitive, bool returnFirstInstance, int depth)
        {
            return FindKeyValuePair(jsonData, new string[0], key, value, valueType, caseSensitive, returnFirstInstance, depth);
        }

        private static List<string[]> FindKeyValuePair(string jsonData, string[] keysOrIndices, string key, string value, ValueType valueType, bool caseSensitive, bool returnFirstInstance, int depth)
        {
            var result = new List<string[]>();
            if (depth == 0)
            {
                return result;
            }

            if (GetValueType(jsonData) == ValueType.Object)
            {
                var dict = DeserializeObject(jsonData);

                foreach (var kvp in dict)
                {
                    if (CheckIfKeyValuePairFulfillsRequirements(kvp, key, value, valueType, caseSensitive))
                    {
                        result.Add(AddKeyOrIndex(keysOrIndices, kvp.Key));
                    }
                }
            }

            return result;
        }

        private static bool CheckIfKeyValuePairFulfillsRequirements(KeyValuePair<string, string> kvp, string key, string value, ValueType valueType, bool caseSensitive)
        {
            if (key != null)
            {
                var k = kvp.Key;
                if (k == null)
                {
                    return false;
                }

                if (!caseSensitive)
                {
                    key = key.ToLower();
                    k = k.ToLower();
                }

                k = SerializeStringIfNotString(k);
                key = SerializeStringIfNotString(key);

                if (k != key)
                {
                    return false;
                }
            }

            if (value != null)
            {
                var v = kvp.Value;
                if (v == null)
                {
                    return false;
                }

                if (!caseSensitive)
                {
                    value = value.ToLower();
                    v = v.ToLower();
                }

                v = SerializeStringIfInvalid(v);
                value = SerializeStringIfInvalid(value);

                if (v != value)
                {
                    return false;
                }
            }

            if (valueType != ValueType.Unspecified)
            {
                if (GetValueType(kvp.Value) != valueType)
                {
                    return false;
                }
            }

            return true;
        }


        



    }
}
