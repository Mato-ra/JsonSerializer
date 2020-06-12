using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSerializer
{
    public static partial class Json
    {
        public static string DeserializeString(string jsonString)
        {
            var result = String.Empty;

            if (jsonString[0] == '"')
            {
                jsonString = jsonString.Substring(1);
            }
            if (jsonString[jsonString.Length - 1] == '"')
            {
                jsonString = jsonString.Substring(0, jsonString.Length - 1);
            }

            for (int i = 0; i < jsonString.Length; i++)
            {
                var character = jsonString[i];

                if (character == '\\')
                {
                    if (i < jsonString.Length - 1)
                    {
                        var nextCharacter = jsonString[i + 1];
                        switch (nextCharacter)
                        {
                            case 't':
                                result += Convert.ToChar(9);
                                i += 2;
                                continue;
                            case 'n':
                                result += Convert.ToChar(10);
                                i += 2;
                                continue;
                            case 'r':
                                result += Convert.ToChar(13);
                                i += 2;
                                continue;
                        }
                    }

                    i++;
                    continue;
                }

                result += character;
            }



            return result;
        }


        public static bool TryDeserializeNumber(string stringValue, out double output)
        {
            double result;
            if (double.TryParse(stringValue, out result))
            {
                output = result;
                return true;
            };

            stringValue = stringValue.Replace(',', '.');

            if (double.TryParse(stringValue, out result))
            {
                output = result;
                return true;
            };

            output = 0;
            return false;
        }

        public static bool TryDeserializeNumber(string stringValue, bool round, out int output)
        {
            double d;
            if (TryDeserializeNumber(stringValue, out d))
            {
                if (round)
                {
                    d = Math.Round(d);
                }

                output = Convert.ToInt32(d);
                return true;
            }

            output = 0;
            return false;
        }

        public static bool TryDeserializeNumber(string stringValue, out int output)
        {
            int result;

            if (int.TryParse(stringValue, out result))
            {
                output = result;
                return true;
            }

            output = 0;
            return false;
        }

        public static int DeserializeNumber(string stringValue, bool round, int defaultValue)
        {
            int result;
            if (TryDeserializeNumber(stringValue, round, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        public static int DeserializeNumber(string stringValue, int defaultValue)
        {
            int result;
            if (TryDeserializeNumber(stringValue, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        public static double DeserializeNumber(string stringValue, double defaultValue)
        {
            double result;
            if (TryDeserializeNumber(stringValue, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }


        public static KeyValuePair<string, string> DeserializeKeyValuePair(string jsonData)
        {
            jsonData = RemoveFormatting(jsonData);
            var i = jsonData.IndexOf(',');
            var result = new KeyValuePair<string, string>(jsonData.Substring(0, i - 1), jsonData.Substring(i + 2));
            return result;
        }



        public static string[] DeserializeArray(string jsonArray)
        {
            var result = new List<string>();
            var bracket = 0;
            var entryIndexArray = new int[2];
            var arrayIndex = 0;
            var escape = false;

            jsonArray = RemoveFormatting(jsonArray);

            if (jsonArray == "[]")
            {
                return result.ToArray();
            }

            for (int i = 0; i < jsonArray.Length; i++)
            {
                var character = jsonArray[i];

                if (character == '"')
                {
                    escape = !escape;
                }

                if (escape)
                {
                    continue;
                }

                if (character == ',')
                {
                    if (bracket == 1 && arrayIndex == 1)
                    {
                        entryIndexArray[arrayIndex] = i - 1;
                        var s = jsonArray.Substring(entryIndexArray[0], entryIndexArray[1] - entryIndexArray[0] + 1);
                        if (s.Length > 0)
                        {
                            result.Add(s);
                        }
                        entryIndexArray[0] = i + 1;
                    }
                }

                if (character == '{' || character == '[')
                {
                    if (bracket == 0)
                    {
                        entryIndexArray[arrayIndex] = i + 1;
                        arrayIndex += 1;
                    }
                    bracket += 1;
                }
                if (character == '}' || character == ']')
                {
                    bracket -= 1;
                    if (bracket == 0)
                    {
                        entryIndexArray[arrayIndex] = i - 1;
                        arrayIndex += 1;
                    }
                }

                if (arrayIndex == 2)
                {
                    var s = jsonArray.Substring(entryIndexArray[0], entryIndexArray[1] - entryIndexArray[0] + 1);
                    if (s.Length > 0)
                    {
                        result.Add(s);
                    }
                    arrayIndex = 0;
                }

            }

            return result.ToArray();
        }


        public static Dictionary<string, string> DeserializeObject(string jsonObject)
        {
            var result = new Dictionary<string, string>();
            var bracket = 0;
            var kvpIndexArray = new int[4];
            var arrayIndex = 0;
            var escape = false;

            jsonObject = RemoveFormatting(jsonObject);

            if (jsonObject == "{}")
            {
                return result;
            }

            for (int i = 0; i < jsonObject.Length; i++)
            {
                var character = jsonObject[i];

                if (character == '\\')
                {
                    i++;
                    continue;
                }

                if (character == '"')
                {
                    escape = !escape;
                    if (bracket == 1 && arrayIndex < 2)
                    {
                        kvpIndexArray[arrayIndex] = i;
                        arrayIndex += 1;
                    }
                }

                if (escape)
                {
                    continue;
                }

                if (character == ':')
                {
                    if (bracket == 1 && arrayIndex == 2)
                    {
                        kvpIndexArray[arrayIndex] = i + 2;
                        arrayIndex += 1;
                    }
                }

                if (character == ',')
                {
                    if (bracket == 1 && arrayIndex == 3)
                    {
                        kvpIndexArray[arrayIndex] = i - 1;
                        arrayIndex += 1;
                    }
                }

                if (character == '{' || character == '[')
                {
                    bracket += 1;
                }
                if (character == '}' || character == ']')
                {
                    bracket -= 1;
                    if (bracket == 0)
                    {
                        kvpIndexArray[arrayIndex] = i - 1;
                        arrayIndex += 1;
                    }
                }

                if (arrayIndex == 4)
                {
                    result.Add(jsonObject.Substring(kvpIndexArray[0], kvpIndexArray[1] - kvpIndexArray[0] + 1), jsonObject.Substring(kvpIndexArray[2], kvpIndexArray[3] - kvpIndexArray[2] + 1));
                    arrayIndex = 0;
                }

            }

            return result;
        }


    }
}
