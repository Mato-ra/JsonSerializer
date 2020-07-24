using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JsonSerializer
{
    public static partial class Json
    {
        public static string AddKeyValuePair(string jsonObject, string key, string value, bool overwrite)
        {
            return AddKeyValuePair(jsonObject, key, value, overwrite, true);
        }

        public static string AddKeyValuePair(string jsonObject, KeyValuePair<string, string> kvp, bool overwrite)
        {
            return AddKeyValuePair(jsonObject, kvp.Key, kvp.Value, overwrite, true);
        }

        private static string AddKeyValuePair(string jsonObject, string key, string value, bool overwrite, bool addKeyValuePair)
        {
            if (CheckValueType(jsonObject) != ValueType.Object)
            {
                return jsonObject;
            }

            key = SerializeStringIfNotString(key);

            if (CheckValueType(value) == ValueType.Invalid)
            {
                value = SerializeString(value);
            }

            var dict = DeserializeObject(jsonObject);
            AddKeyValuePair(dict, key, value, overwrite);

            return SerializeObject(dict);
        }

        public static Dictionary<string, string> AddKeyValuePair(Dictionary<string, string> jsonObject, string key, string value, bool overwrite)
        {
            return AddKeyValuePair(jsonObject, new KeyValuePair<string, string>(key, value), overwrite, true);
        }

        public static Dictionary<string, string> AddKeyValuePair(Dictionary<string, string> jsonObject, KeyValuePair<string, string> kvp, bool overwrite)
        {
            return AddKeyValuePair(jsonObject, kvp, overwrite, true);
        }

        private static Dictionary<string, string> AddKeyValuePair(Dictionary<string, string> jsonObject, KeyValuePair<string, string> kvp, bool overwrite, bool addKeyValuePair)
        {
            if (overwrite || GetKvpValue(jsonObject, kvp.Key, true) == null)
            {
                jsonObject[kvp.Key] = kvp.Value;
            }

            return jsonObject;
        }

        public static string AddArrayEntry(string jsonArray, string value)
        {
            if (jsonArray == null || value == null)
            {
                return jsonArray;
            }

            if (CheckValueType(jsonArray) != ValueType.Array)
            {
                return jsonArray;
            }

            return SerializeArray(AddArrayEntry(DeserializeArray(jsonArray), value));
        }

        public static string[] AddArrayEntry(string[] jsonArray, string value)
        {
            if (jsonArray == null || value == null)
            {
                return jsonArray;
            }

            if (CheckValueType(value) == ValueType.Invalid)
            {
                value = SerializeString(value);
            }

            var arr = (jsonArray).ToList<string>();
            arr.Add(value);
            return arr.ToArray();
        }

        public static string AddValue(string jsonData, string[] keysOrIndices, string value, bool caseSensitive, bool overwrite, bool addKeyValuePair, bool addArrayEntry)
        {
            if (CheckValueType(value) == ValueType.Invalid)
            {
                value = SerializeString(value);
            }

            if (keysOrIndices.Length > 1)
            {
                if (CheckValueType(jsonData) == ValueType.Object)
                {
                    var key = keysOrIndices[0];
                    var a = RemoveKeyOrIndex(keysOrIndices, 0);
                    var oldValue = GetValue(jsonData, key, caseSensitive);
                    var newValue = AddValue(jsonData, a, value, caseSensitive, overwrite, addKeyValuePair, addArrayEntry);
                    return AddKeyValuePair(jsonData, key, newValue, true);
                }

                if (CheckValueType(jsonData) == ValueType.Array)
                {
                    var index = keysOrIndices[0];
                    var a = RemoveKeyOrIndex(keysOrIndices, 0);
                    var oldValue = GetValue(jsonData, index, caseSensitive);
                    var newValue = AddValue(jsonData, a, value, caseSensitive, overwrite, addKeyValuePair, addArrayEntry);
                    return EditArrayEntry(jsonData, index, newValue);
                }
            }


            if (CheckValueType(jsonData) == ValueType.Object)
            {
                var key = keysOrIndices[0];
                if (addArrayEntry)
                {
                    var targetValue = GetValue(jsonData, key, true);
                    if (CheckValueType(targetValue) == ValueType.Array)
                    {
                        return AddKeyValuePair(jsonData, key, AddArrayEntry(targetValue, value), true);
                    }
                }

                if (!addKeyValuePair)
                {
                    if (GetValue(jsonData, key, caseSensitive) != null){
                        return AddKeyValuePair(jsonData, key, value, overwrite);
                    }
                    else
                    {
                        return jsonData;
                    }
                }

                return AddKeyValuePair(jsonData, key, value, overwrite);
            }

            if (CheckValueType(jsonData) == ValueType.Array)
            {
                var index = keysOrIndices[0];
                return EditArrayEntry(jsonData, index, value);
            }

            return jsonData;
        }

        public static string MergeObjects(string[] jsonObjects, bool overwrite)
        {
            if (jsonObjects == null || jsonObjects.Length == 0)
            {
                return "{}";
            }

            var result = DeserializeObject(jsonObjects[0]);

            for(int i = 1; i < jsonObjects.Length; i++)
            {
                var o = DeserializeObject(jsonObjects[i]);
                foreach(var kvp in o)
                {
                    AddKeyValuePair(result, kvp, overwrite);
                }
            }

            return SerializeObject(result);
        }
    }
}
