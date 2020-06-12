using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSerializer
{
    public static partial class Json
    {
        public static string SerializeValue(object value)
        {
            if (value == null)
            {
                return "null";
            }
            if (value.GetType() == typeof(string))
            {
                return SerializeString((string)value);
            }

            if (value.GetType() == typeof(bool))
            {
                return SerializeBoolean((bool)value);
            }

            if (value.GetType() == typeof(int))
            {
                return SerializeNumber((int)value);
            }
            if (value.GetType() == typeof(float))
            {
                return SerializeNumber((float)value);
            }
            if (value.GetType() == typeof(double))
            {
                return SerializeNumber((double)value);
            }


            return null;
        }


        public static string SerializeStringIfInvalid(string value)
        {
            if (GetValueType(value) == ValueType.Invalid)
            {
                return SerializeString(value);
            }
            else
            {
                return value;
            }
        }

        public static string SerializeStringIfNotString(string value)
        {
            if (GetValueType(value) != ValueType.String)
            {
                return SerializeString(value);
            }
            else
            {
                return value;
            }
        }

        public static string SerializeString(string stringValue)
        {
            if (stringValue == null)
            {
                return SerializeNull();
            }

            var result = stringValue
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\t", "\\t")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");


            return $"\"{result}\"";
        }

        public static string SerializeNumber(double value)
        {
            return value.ToString().Replace(',', '.');
        }

        public static string SerializeNumber(float value)
        {
            return value.ToString().Replace(',', '.');
        }

        public static string SerializeNumber(int value)
        {
            return value.ToString();
        }

        public static string SerializeBoolean(bool value)
        {
            if (value)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public static string SerializeKeyValuePair(string key, string value)
        {
            key = SerializeStringIfNotString(key);
            value = SerializeStringIfInvalid(value);

            return $"{key}: {value}";
        }

        public static string SerializeKeyValuePair(KeyValuePair<string, string> keyValuePair)
        {
            return SerializeKeyValuePair(keyValuePair.Key, keyValuePair.Value);
        }

        public static string SerializeArray(string[] array)
        {
            if (array == null)
            {
                return SerializeNull();
            }

            if (array.Length == 0)
            {
                return "[]";
            }

            var entries = new List<string>();
            foreach (var entry in array)
            {
                entries.Add(entry);
            }

            return $"[{String.Join(",", entries)}]";
        }

        public static string SerializeObject(Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                return SerializeNull();
            }

            if (dictionary.Count == 0)
            {
                return "{}";
            }

            var entries = new List<string>();
            foreach (var entry in dictionary)
            {
                entries.Add(SerializeKeyValuePair(entry));
            }

            return $"{{{String.Join(",", entries)}}}";
        }




        public static string SerializeNull()
        {
            return "null";
        }
    }
}
