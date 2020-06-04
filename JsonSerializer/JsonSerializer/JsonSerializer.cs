using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JsonSerializer
{
    public static class Json
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

        public static string AddFormatting(string jsonData)
        {
            int tabs = 0;
            bool escape = false;

            for(int i = 0; i < jsonData.Length; i++)
            {
                var character = jsonData[i];
                if (character == '"')
                {
                    escape = !escape;
                }

                if (escape)
                {
                    continue;
                }

                if (character == '{' || character == '[')
                {
                    tabs += 1;

                    jsonData = jsonData.Insert(i + 1, Environment.NewLine);
                }

                if (character == ',')
                {
                    jsonData = jsonData.Insert(i + 1, Environment.NewLine);
                }

                if (character == '}' || character == ']')
                {
                    tabs -= 1;

                    jsonData = jsonData.Insert(i , Environment.NewLine);
                    i += Environment.NewLine.Length;

                    for (int j = 0; j < tabs; j++)
                    {
                        jsonData = jsonData.Insert(i, Convert.ToChar(9).ToString());
                    }

                    i += tabs;
                }

                if (character == Convert.ToChar(13))
                {
                    if (jsonData[i + 1] == Convert.ToChar(10))
                    {
                        i++;
                    }
                    for(int j = 0; j < tabs; j++)
                    {
                        jsonData = jsonData.Insert(i + 1, Convert.ToChar(9).ToString());
                    }
                }
            }

            return jsonData;
        }

        public static string RemoveFormatting(string jsonData)
        {
            var escape = false;
            for(int i = 0; i < jsonData.Length; i++)
            {
                var character = jsonData[i];

                if (character == '"')
                {
                    escape = !escape;
                }

                if (escape)
                {
                    continue;
                }

                if (character == '/')
                {
                    i++;
                    continue;
                }

                if (character == ':')
                {
                    if (i + 1 < jsonData.Length)
                    {
                        if (jsonData[i + 1] == ' ')
                        {
                            i++;
                            continue;
                        }
                    }
                }

                if (character == ' ')
                {
                    jsonData = jsonData.Remove(i, 1);
                    i--;
                    continue;
                }
            }

            return jsonData.Replace(Convert.ToChar(10).ToString(), null).Replace(Convert.ToChar(13).ToString(), null).Replace(Convert.ToChar(9).ToString(), null);
        }

        public static string SerializeString(string stringValue)
        {
            var result = stringValue
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace(Convert.ToChar(9).ToString(), "\\t")
                .Replace(Convert.ToChar(10).ToString(), "\\n")
                .Replace(Convert.ToChar(13).ToString(), "\\r");


            return $"\"{result}\"";
        }

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
            }

            

            return result;
        }

        public static string SerializeKeyValuePair(KeyValuePair<string, string> keyValuePair)
        {
            return $"{keyValuePair.Key}: {keyValuePair.Value}";
        }

        public static KeyValuePair<string, string> DeserializeKeyValuePair(string jsonData)
        {
            var i = jsonData.IndexOf(',');
            var result = new KeyValuePair<string, string>(jsonData.Substring(0, i - 1), jsonData.Substring(i + 2));
            return result;
        }

        public static string SerializeArray(string[] array)
        {
            var entries = new List<string>();
            foreach(var entry in array)
            {
                entries.Add(entry);
            }

            return $"[{String.Join(",", entries)}]";
        }

        public static string[] DeserializeArray(string jsonArray)
        {
            var result = new List<string>();
            var bracket = 0;
            var entryIndexArray = new int[2];
            var arrayIndex = 0;
            var escape = false;

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

        public static string SerializeObject(Dictionary<string, string> dictionary)
        {
            var entries = new List<string>();
            foreach (var entry in dictionary)
            {
                entries.Add(SerializeKeyValuePair(entry));
            }

            return $"{{{String.Join(",", entries)}}}";
        }

        public static Dictionary<string, string> DeserializeObject(string jsonObject)
        {
            var result = new Dictionary<string, string>();
            var bracket = 0;
            var kvpIndexArray = new int[4];
            var arrayIndex = 0;

            for(int i = 0; i < jsonObject.Length; i++)
            {
                var character = jsonObject[i];


                if (character == ':')
                {
                    if (bracket == 1 && arrayIndex == 2)
                    {
                        kvpIndexArray[arrayIndex] = i + 2;
                        arrayIndex += 1;
                    }
                }

                if (character == '"')
                {
                    if (bracket == 1 && arrayIndex < 2)
                    {
                        kvpIndexArray[arrayIndex] = i;
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
