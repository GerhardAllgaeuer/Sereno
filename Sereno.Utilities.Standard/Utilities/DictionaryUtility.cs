using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Utilities
{
    public class DictionaryUtility
    {


        /// <summary>
        /// Dictionary aus einer Liste erstellen
        /// </summary>
        public static Dictionary<TKey, TSource> CreateDictionaryFromList<TSource, TKey>(List<TSource> list, string keyPropertyName)
                            where TKey : class
        {
            var dictionary = new Dictionary<TKey, TSource>();
            var propertyInfo = typeof(TSource).GetProperty(keyPropertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{keyPropertyName}' not found on type '{typeof(TSource).Name}'.");
            }

            foreach (var item in list)
            {
                var key = propertyInfo.GetValue(item) as TKey;

                // Überprüfen, ob der Schlüssel nicht null ist
                if (key != null)
                {
                    dictionary[key] = item; // Verwendet Indexer, um Duplikate zu überschreiben
                }
            }

            return dictionary;
        }
    }
}
