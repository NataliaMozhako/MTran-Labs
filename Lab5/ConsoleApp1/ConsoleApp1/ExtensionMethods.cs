using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    static class ExtensionMethods
    {
        public static bool TryAdd<TK, TV>(this Dictionary<TK, TV> dictionary, TK key, TV value)
        {
            bool containsKey = dictionary.ContainsKey(key);
            if (!containsKey)
                dictionary.Add(key, value);
            return !containsKey;
        }

        public static TV GetOrDefault<TK, TV>(this Dictionary<TK, TV> dictionary, TK key, TV defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return defaultValue;
        }
    }
}
