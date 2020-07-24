using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JsonSerializer
{
    public static partial class Json
    {
        /// <summary>
        /// Searches a json string for objects filtered by their key value pairs.
        /// </summary>
        /// <param name="jsonData">the json string which will be examined</param>
        /// <param name="key">the desired key of the key value pair contained in the objects</param>
        /// <param name="value">the desired value of the key value pair contained in the objects</param>
        /// <returns>list of indices to the desired objects</returns>
        public static List<string[]> FindObjects(string jsonData, string key, string value)
        {
            return FindObjects(jsonData, key, value, ValueType.Unspecified, ArrayEntryOrKvpValue.Either, false, false, -1);
        }

        /// <summary>
        /// Searches a json string for objects filtered by their key value pairs.
        /// </summary>
        /// <param name="jsonData">the json string which will be examined</param>
        /// <param name="key">the desired key of the key value pair contained in the objects</param>
        /// <param name="valueType">value type of the key value pair's value</param>
        /// <returns>list of indices to the desired objects</returns>
        public static List<string[]> FindObjects(string jsonData, string key, ValueType valueType)
        {
            return FindObjects(jsonData, key, null, valueType, ArrayEntryOrKvpValue.Either, false, false, -1);
        }

        /// <summary>
        /// Searches a json string for objects filtered by their key value pairs.
        /// </summary>
        /// <param name="jsonData">the json string which will be examined</param>
        /// <param name="key">the desired key of the key value pair contained in the objects</param>
        /// <param name="value">the desired value of the key value pair contained in the objects</param>
        /// <param name="valueType">value type of the key value pair's value</param>
        /// <returns>list of indices to the desired objects</returns>
        public static List<string[]> FindObjects(string jsonData, string key, string value, ValueType valueType)
        {
            return FindObjects(jsonData, key, value, valueType, ArrayEntryOrKvpValue.Either, false, false, -1);
        }

        /// <summary>
        /// Searches a json string for objects filtered by their key value pairs.
        /// </summary>
        /// <param name="jsonData">the json string which will be examined</param>
        /// <param name="key">the desired key of the key value pair contained in the objects</param>
        /// <param name="value">the desired value of the key value pair contained in the objects</param>
        /// <param name="valueType">value type of the key value pair's value</param>
        /// <param name="caseSensitive">key and value filter is case sensitive</param>
        /// <returns>list of indices to the desired objects</returns>
        public static List<string[]> FindObjects(string jsonData, string key, string value, ValueType valueType, bool caseSensitive)
        {
            return FindObjects(jsonData, key, value, valueType, ArrayEntryOrKvpValue.Either, caseSensitive, false, -1);
        }

        /// <summary>
        /// Searches a json string for objects filtered by their key value pairs.
        /// </summary>
        /// <param name="jsonData">the json string which will be examined</param>
        /// <param name="key">the desired key of the key value pair contained in the objects</param>
        /// <param name="value">the desired value of the key value pair contained in the objects</param>
        /// <param name="valueType">value type of the key value pair's value</param>
        /// <param name="arrayEntryOrKvpValue">//TODO</param>
        /// <param name="caseSensitive">key and value filter is case sensitive</param>
        /// <param name="returnFirstInstance">stops searching for more objects after finding one</param>
        /// <param name="depth">maximum amount of indices</param>
        /// <returns>list of indices to the desired objects</returns>
        public static List<string[]> FindObjects(string jsonData, string key, string value, ValueType valueType, ArrayEntryOrKvpValue arrayEntryOrKvpValue, bool caseSensitive, bool returnFirstInstance, int depth)
        {
            var filters = new KeyValuePairFilter[] { new KeyValuePairFilter(key, value, valueType, caseSensitive, true, arrayEntryOrKvpValue) };
            return FindObjects(jsonData, new string[0], filters, returnFirstInstance, depth);
        }

        /// <summary>
        /// Searches a json string for objects filtered by their key value pairs.
        /// </summary>
        /// <param name="jsonData">the json string which will be examined</param>
        /// <param name="filters">an array of key value pair requirements</param>
        /// <param name="returnFirstInstance">stops searching for more objects after finding one</param>
        /// <param name="depth">maximum amount of indices</param>
        /// <returns>list of indices to the desired objects</returns>
        public static List<string[]> FindObjects(string jsonData, KeyValuePairFilter[] filters, bool returnFirstInstance, int depth)
        {
            return FindObjects(jsonData, new string[0], filters, returnFirstInstance, depth);
        }

        /// <summary>
        /// Searches a json string for objects filtered by their key value pairs.
        /// </summary>
        /// <param name="jsonData">the json string which will be examined</param>
        /// <param name="filters">an array of key value pair requirements</param>
        /// <param name="returnFirstInstance">stops searching for more objects after finding one</param>
        /// <returns>list of indices to the desired objects</returns>
        public static List<string[]> FindObjects(string jsonData, KeyValuePairFilter[] filters, bool returnFirstInstance)
        {
            return FindObjects(jsonData, new string[0], filters, returnFirstInstance, -1);
        }

        private static List<string[]> FindObjects(string jsonData, string[] keysOrIndices, KeyValuePairFilter[] filters, bool returnFirstInstance, int depth)
        {
            return FindObjects(jsonData, keysOrIndices, filters, returnFirstInstance, depth, CheckValueType(jsonData) == ValueType.Array);
        }

        private static List<string[]> FindObjects(string jsonData, string[] keysOrIndices, KeyValuePairFilter[] filters, bool returnFirstInstance, int depth, bool arrayEntry)
        {
            var result = new List<string[]>();
            if (depth == 0)
            {
                return result;
            }

            if (CheckValueType(jsonData) == ValueType.Object)
            {
                var dict = DeserializeObject(jsonData);

                if (CheckIfObjectFulfillsRequirements(dict, filters, arrayEntry))
                {
                    result.Add(keysOrIndices);
                }

                foreach (var kvp in dict)
                {
                    if (CheckValueType(kvp.Value) == ValueType.Object || CheckValueType(kvp.Value) == ValueType.Array)
                    {
                        result.AddRange(FindObjects(GetValue(jsonData, kvp.Key, true), AddKeyOrIndex(keysOrIndices, kvp.Key), filters, returnFirstInstance, depth - 1, false));
                    }
                }
            }

            if (CheckValueType(jsonData) == ValueType.Array)
            {
                var arr = DeserializeArray(jsonData);

                for (int i = 0; i < arr.Length; i++)
                {
                    var entry = arr[i];
                    if (CheckValueType(entry) == ValueType.Object || CheckValueType(entry) == ValueType.Array)
                    {
                        result.AddRange(FindObjects(GetValue(jsonData, i.ToString(), true), AddKeyOrIndex(keysOrIndices, i.ToString()), filters, returnFirstInstance, depth - 1, true));
                    }
                }
            }

            return result;
        }

        private static bool CheckIfObjectFulfillsRequirements(Dictionary<string, string> objectDictionary, KeyValuePairFilter[] filters, bool arrayEntry)
        {
            if (filters == null || objectDictionary == null)
            {
                return false;
            }

            foreach (var filter in filters)
            {
                if (!CheckIfObjectFulfillsRequirements(objectDictionary, filter, arrayEntry))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckIfObjectFulfillsRequirements(Dictionary<string, string> objectDictionary, KeyValuePairFilter filter, bool arrayEntry)
        {
            var key = filter.key;
            var value = filter.value;
            var valueType = filter.valueType;
            var caseSensitive = filter.caseSensitive;
            var keyIsMandatory = filter.keyIsMandatory;
            var arrayEntryOrKvpValue = filter.arrayEntryOrKvpValue;

            if ((arrayEntry && arrayEntryOrKvpValue == ArrayEntryOrKvpValue.KvpValue) || (!arrayEntry && arrayEntryOrKvpValue == ArrayEntryOrKvpValue.ArrayEntry))
            {
                return false;
            }

            foreach (var kvp in objectDictionary)
            {
                if (key != null)
                {
                    if (CheckIfKeyFulfillsRequirements(kvp.Key, filter))
                    {
                        if (value != null)
                        {
                            return CheckIfValueFulfillsRequirements(kvp.Value, filter);
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (CheckIfValueFulfillsRequirements(kvp.Value, filter))
                    {
                        return true;
                    }
                }
            }

            return !keyIsMandatory;
        }

        private static bool CheckIfKeyFulfillsRequirements(string key, KeyValuePairFilter filter)
        {
            if (filter.key != null)
            {
                var k = filter.key;

                if (key == null)
                {
                    return false;
                }


                if (!filter.caseSensitive)
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

                return true;
            }

            return false;
        }

        private static bool CheckIfValueFulfillsRequirements(string value, KeyValuePairFilter filter)
        {
            var v = value;
            value = filter.value;
            var valueType = filter.valueType;
            var caseSensitive = filter.caseSensitive;

            if (CheckValueType(value) == ValueType.Object)
            {
                if (CheckValueType(v) == ValueType.Number && (valueType == ValueType.Number || valueType == ValueType.Unspecified))
                {
                    var f = DeserializeObject(value);
                    foreach (var fkvp in f)
                    {
                        if (DeserializeString(fkvp.Key.ToLower()) == "gt")
                        {
                            double a;
                            double w;
                            if (Double.TryParse(fkvp.Value, out a) && Double.TryParse(v, out w))
                            {
                                if (w < a)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (DeserializeString(fkvp.Key.ToLower()) == "lt")
                        {
                            double a;
                            double w;
                            if (Double.TryParse(fkvp.Value, out a) && Double.TryParse(v, out w))
                            {
                                if (w > a)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    return true;
                }
            }


            if (value != null)
            {
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
                if (CheckValueType(v) != valueType)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class KeyValuePairFilter{
        public string key;
        public string value;
        public Json.ValueType valueType;
        public bool caseSensitive;
        public bool keyIsMandatory;
        public Json.ArrayEntryOrKvpValue arrayEntryOrKvpValue;

        

        public KeyValuePairFilter(string key, string value, Json.ValueType valueType, bool caseSensitive, bool keyIsMandatory, Json.ArrayEntryOrKvpValue arrayEntryOrKvpValue)
        {
            this.key = key;
            this.value = value;
            this.valueType = valueType;
            this.caseSensitive = caseSensitive;
            this.keyIsMandatory = keyIsMandatory;
            this.arrayEntryOrKvpValue = arrayEntryOrKvpValue;
        }
    }
}
