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
            if (GetValueType(jsonObject) != ValueType.Object)
            {
                return jsonObject;
            }

            key = SerializeStringIfNotString(key);

            if (GetValueType(value) == ValueType.Invalid)
            {
                value = SerializeString(value);
            }

            var dict = DeserializeObject(jsonObject);
            if (overwrite || GetKvpValue(jsonObject, key, true) == null)
            {
                dict.Add(key, value);
            }

            return SerializeObject(dict);
        }

        public static string AddArrayEntry(string jsonArray, string value)
        {
            if (jsonArray == null || value == null)
            {
                return jsonArray;
            }

            if (GetValueType(jsonArray) != ValueType.Array)
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

            if (GetValueType(value) == ValueType.Invalid)
            {
                value = SerializeString(value);
            }

            var arr = (jsonArray).ToList<string>();
            arr.Add(value);
            return arr.ToArray();
        }

        public static string AddValue(string jsonData, string[] keysOrIndices, string value, bool caseSensitive, bool overwrite, bool addArrayEntry)
        {
            if (GetValueType(value) == ValueType.Invalid)
            {
                value = SerializeString(value);
            }

            if (keysOrIndices.Length > 1)
            {
                if (GetValueType(jsonData) == ValueType.Object)
                {
                    var key = keysOrIndices[0];
                    var a = RemoveKeyOrIndex(keysOrIndices, 0);
                    var oldValue = GetValue(jsonData, key, caseSensitive);
                    var newValue = AddValue(jsonData, a, value, caseSensitive, overwrite, addArrayEntry);
                    return AddKeyValuePair(jsonData, key, newValue, true);
                }

                if (GetValueType(jsonData) == ValueType.Array)
                {
                    var index = keysOrIndices[0];
                    var a = RemoveKeyOrIndex(keysOrIndices, 0);
                    var oldValue = GetValue(jsonData, index, caseSensitive);
                    var newValue = AddValue(jsonData, a, value, caseSensitive, overwrite, addArrayEntry);
                    return EditArrayEntry(jsonData, index, newValue);
                }
            }


            if (GetValueType(jsonData) == ValueType.Object)
            {
                var key = keysOrIndices[0];
                if (addArrayEntry)
                {
                    var targetValue = GetValue(jsonData, key, true);
                    if (GetValueType(value) == ValueType.Array)
                    {
                        return AddArrayEntry(jsonData, targetValue);
                    }
                }

                return AddKeyValuePair(jsonData, key, value, overwrite);
            }

            if (GetValueType(jsonData) == ValueType.Array)
            {
                var index = keysOrIndices[0];
                return EditArrayEntry(jsonData, index, value);
            }

            return jsonData;
        }


    }
}
