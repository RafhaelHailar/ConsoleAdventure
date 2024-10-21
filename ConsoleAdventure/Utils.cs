
using System;
using System.Collections.Generic;

namespace ConsoleAdventureUtils
{
    public class BiDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> keyToValue = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TValue, TKey> valueToKey = new Dictionary<TValue, TKey>();


        // Add a key-value pair to the BiDictionary
        public void Add(TKey key, TValue value)
        {
            if (keyToValue.ContainsKey(key) || valueToKey.ContainsKey(value))
            {
                throw new ArgumentException("Duplicate key or value.");
            }

            keyToValue[key] = value;
            valueToKey[value] = key;
        }

        // Get value by key
        public TValue GetByKey(TKey key) => keyToValue.TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();

        // Get key by value
        public TKey GetByValue(TValue value) => valueToKey.TryGetValue(value, out var key) ? key : throw new KeyNotFoundException();

        // Indexer to access value by key
        public TValue this[TKey key] => GetByKey(key);

        // Indexer to access key by value
        public TKey this[TValue value] => GetByValue(value);

        // Check if a key exists
        public bool ContainsKey(TKey key) => keyToValue.ContainsKey(key);

        // Check if a value exists
        public bool ContainsValue(TValue value) => valueToKey.ContainsKey(value);
    }
}
