using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSerializer
{
    public static partial class Json
    {
        public static string AddFormatting(string jsonData)
        {
            int tabs = 0;
            bool escape = false;

            jsonData = RemoveFormatting(jsonData);

            for (int i = 0; i < jsonData.Length; i++)
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

                    jsonData = jsonData.Insert(i, Environment.NewLine);
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
                    for (int j = 0; j < tabs; j++)
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

            jsonData = jsonData.Replace(Convert.ToChar(10).ToString(), null).Replace(Convert.ToChar(13).ToString(), null).Replace(Convert.ToChar(9).ToString(), null);

            for (int i = 0; i < jsonData.Length; i++)
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
                        else
                        {
                            jsonData = jsonData.Insert(i + 1, " ");
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

            return jsonData;
        }

    }
}
