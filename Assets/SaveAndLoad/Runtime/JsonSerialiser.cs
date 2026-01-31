using System;
using UnityEngine;

namespace SaveAndLoad {
    [Serializable]
    internal class JsonSerialiser : ISerialiser {
        public string Serialise<T>(T obj) {
            return JsonUtility.ToJson(obj, true);
        }
        
        public T Deserialise<T>(string data) {
            return JsonUtility.FromJson<T>(data);
        }
    }
}
