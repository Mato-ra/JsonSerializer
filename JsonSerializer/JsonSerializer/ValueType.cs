using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSerializer
{
    public static partial class Json
    {
        public enum ValueType
        {
            Unspecified,
            String,
            Number,
            Array,
            Object,
            Boolean,
            Null,
            Invalid,
        }

        public static ValueType CheckValueType(string value)
        {
            if (value == null)
            {
                return ValueType.Null;
            }

            while (value.Length > 0 && value[0] == ' ')
            {
                value = value.Substring(1);
            }

            while (value.Length > 0 && value[value.Length - 1] == ' ')
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"')
            {
                return ValueType.String;
            }

            if (value.Length > 1 && value[0] == '{' && value[value.Length - 1] == '}')
            {
                return ValueType.Object;
            }

            if (value.Length > 1 && value[0] == '[' && value[value.Length - 1] == ']')
            {
                return ValueType.Array;
            }

            var d = 0d;
            if (TryDeserializeNumber(value, out d))
            {
                return ValueType.Number;
            }

            switch (value)
            {
                case "true": case "false": return ValueType.Boolean;
                case "null": return ValueType.Null;
            }

            return ValueType.Invalid;
        }
    }
}
