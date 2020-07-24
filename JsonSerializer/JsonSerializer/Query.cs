using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSerializer
{
    public static partial class Json
    {
        public static KeyValuePairFilter[] ReadQueryString(string queryString, bool keyIsMandatory, ArrayEntryOrKvpValue arrayEntryOrKvpValue)
        {
            if (queryString == null || queryString.Length == 0)
            {
                return new KeyValuePairFilter[] { new KeyValuePairFilter(null, null, ValueType.Unspecified, false, false, ArrayEntryOrKvpValue.Either) };
            }
            while (queryString[0] == '?')
            {
                queryString = queryString.Substring(1);
                if (queryString.Length == 0)
                {
                    return new KeyValuePairFilter[] { new KeyValuePairFilter(null, null, ValueType.Unspecified, false, false, ArrayEntryOrKvpValue.Either) };
                }
            }

            var result = new List<KeyValuePairFilter>();
            var kvp = String.Empty;
            var li = new List<String[]>();
            var a = queryString.Split(';');
            

            foreach(var b in a)
            {
                for(int i = 0; i < b.Length; i++)
                {
                    if (b[i] == '=')
                    {
                        li.Add(new string[] { b.Substring(0, i), b.Substring(i + 1) });
                        break;
                    }
                }
            }

            foreach(var l in li)
            {
                result.Add(new KeyValuePairFilter(SerializeString(l[0]), SerializeStringIfInvalid(l[1]), ValueType.Unspecified, false, keyIsMandatory, arrayEntryOrKvpValue));
            }

            return result.ToArray();
        }

        public static string QueryJsonData(string jsonData, string queryString, string[] keysOrIndices, int depth, bool caseSensitive, bool keyIsMandatory, ArrayEntryOrKvpValue arrayEntryOrKvpValue)
        {
            var newValue = QueryJsonData(GetValue(jsonData, keysOrIndices, caseSensitive), queryString, depth, caseSensitive, keyIsMandatory, arrayEntryOrKvpValue);
            return EditValue(jsonData, keysOrIndices, newValue, caseSensitive);
        }

        public static string QueryJsonData(string jsonData, string queryString, int depth, bool caseSensitive, bool keyIsMandatory, ArrayEntryOrKvpValue arrayEntryOrKvpValue)
        {
            var paths = FindObjects(jsonData, ReadQueryString(queryString, keyIsMandatory, arrayEntryOrKvpValue), false, depth);
            var result = new Dictionary<string, string>();

            foreach(var p in paths)
            {
                string k = null;
                var isArray = false;
                for(int i = p.Length - 1; i >= 0; i--)
                {
                    if (CheckValueType(p[i]) == ValueType.String)
                    {
                        k = p[i];
                        break;
                    }
                    else
                    {
                        isArray = true;
                    }

                }

                if(k != null)
                {
                    if (!isArray)
                    {
                        result[k] = GetValue(jsonData, p, caseSensitive);
                    }
                    else
                    {
                        string arr = Json.CreateNewArray();
                        if (result.ContainsKey(k))
                        {
                            var a = result[k];
                            if (CheckValueType(a) == ValueType.Array)
                            {
                                arr = a;
                            }                            
                        }

                        arr = AddArrayEntry(arr, GetValue(jsonData, p, caseSensitive));
                        result[k] = arr;
                    }
                }

            }

            return SerializeObject(result);
        }

    }
}
