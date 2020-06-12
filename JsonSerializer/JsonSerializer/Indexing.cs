using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JsonSerializer
{
    public static partial class Json
    {
        private static string[] AddKeyOrIndex(string[] keysOrIndices, string keyOrIndex)
        {
            if (keysOrIndices == null)
            {
                return null;
            }

            if (keyOrIndex == null)
            {
                return keysOrIndices;
            }

            var l = keysOrIndices.ToList<string>();
            l.Add(keyOrIndex);
            return l.ToArray();
        }

        private static string[] RemoveKeyOrIndex(string[] keysOrIndices, int index)
        {
            if (keysOrIndices == null)
            {
                return null;
            }

            var l = keysOrIndices.ToList<string>();
            if (index >= 0 && keysOrIndices.Length > index)
            {
                l.RemoveAt(index);
            }

            return l.ToArray();
        }

        private static string[] CopyKeysOrIndices(string[] keysOrIndices)
        {
            var result = new List<string>();
            foreach (var ki in keysOrIndices)
            {
                result.Add(ki);
            }

            return result.ToArray();
        }
    }
}
