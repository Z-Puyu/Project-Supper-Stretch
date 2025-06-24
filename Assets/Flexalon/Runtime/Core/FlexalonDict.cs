using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flexalon.Runtime.Core
{
    [Serializable]
    internal class FlexalonDict<K, V> : ISerializationCallbackReceiver
    {
        private Dictionary<K, V> _dict = new Dictionary<K, V>();

        [SerializeField]
        private List<K> _keys = new List<K>();

        [SerializeField]
        private List<V> _values = new List<V>();

        public void Add(K key, V value)
        {
            this._dict.Add(key, value);
        }

        public bool TryGetValue(K key, out V value)
        {
            return this._dict.TryGetValue(key, out value);
        }

        public void Clear()
        {
            this._dict.Clear();
        }

        public int Count => this._dict.Count;

        public void OnBeforeSerialize()
        {
            this._keys.Clear();
            this._values.Clear();

            foreach (var kvp in this._dict)
            {
                this._keys.Add(kvp.Key);
                this._values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this._dict.Clear();
            for (int i = 0; i < this._keys.Count; i++)
            {
                this._dict.Add(this._keys[i], this._values[i]);
            }

            this._keys.Clear();
            this._values.Clear();
        }
    }
}