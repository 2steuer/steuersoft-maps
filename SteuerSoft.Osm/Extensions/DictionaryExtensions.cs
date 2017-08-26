using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Osm.Extensions
{
    static class DictionaryExtensions
    {

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (!dict.ContainsKey(key))
            {
                return defaultValue;
            }
            else
            {
                return dict[key];
            }
        }

       
    }
}
