using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace JsonSerializer
{
    public static partial class Json
    {
        

        public static string ReadJsonFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            return RemoveFormatting(File.ReadAllText(path));
        }

        public static bool WriteJsonFile(string path, string jsonData)
        {
            try
            {
                File.WriteAllText(path, AddFormatting(jsonData));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetValue(string jsonData, string keyOrIndex, bool caseSensitive)
        {
            switch (GetValueType(jsonData))
            {
                case ValueType.Object:
                    return GetKvpValue(jsonData, keyOrIndex, caseSensitive);

                case ValueType.Array:
                    return GetArrayEntry(jsonData, keyOrIndex);

                default:
                    return null;
            }
        }

        public static string GetArrayEntry(string jsonArray, string index)
        {
            var arr = DeserializeArray(jsonArray);

            int i = -1;

            if (!TryDeserializeNumber(index, out i))
            {
                return null;
            }

            if (arr.Length > i && i >= 0)
            {
                return arr[i];
            }

            return null;
        }

        public static string GetKvpValue(string jsonObject, string key, bool caseSensitive)
        {
            var kvp = GetKeyValuePair(jsonObject, key, caseSensitive);

            if (kvp.Key != null)
            {
                return kvp.Value;
            }
            else
            {
                return null;
            }
        }

        public static string GetKvpValue(Dictionary<string, string> jsonObject, string key, bool caseSensitive)
        {
            var kvp = GetKeyValuePair(jsonObject, key, caseSensitive);

            if (kvp.Key != null)
            {
                return kvp.Value;
            }
            else
            {
                return null;
            }
        }

        public static KeyValuePair<string, string> GetKeyValuePair(string jsonObject, string key, bool caseSensitive)
        {
            var dict = DeserializeObject(jsonObject);

            return GetKeyValuePair(dict, key, caseSensitive);
        }

        public static KeyValuePair<string, string> GetKeyValuePair(Dictionary<string, string> jsonObject, string key, bool caseSensitive)
        {
            var dict = jsonObject;

            if (!caseSensitive)
            {
                key = key.ToLower();
            }

            foreach (var kvp in dict)
            {
                var k = kvp.Key;
                if (!caseSensitive)
                {
                    k = k.ToLower();
                }

                if (DeserializeString(k) == DeserializeString(key))
                {
                    return kvp;
                }
            }

            return new KeyValuePair<string, string>(null, null);
        }

        public static string CreateNewObject()
        {
            return "{}";
        }

        public static string CreateNewArray()
        {
            return "[]";
        }

        public static string EditArrayEntry(string jsonArray, string index, string value)
        {
            if (GetValueType(jsonArray) != ValueType.Array)
            {
                return jsonArray;
            }

            int i = -1;

            if (!TryDeserializeNumber(index, out i))
            {
                return jsonArray;
            }

            if (GetValueType(value) == ValueType.Invalid)
            {
                value = SerializeString(value);
            }

            var arr = DeserializeArray(jsonArray);

            if (arr.Length > i && i >= 0)
            {
                arr[i] = value;
            }

            return SerializeArray(arr.ToArray());
        }

    }
}
