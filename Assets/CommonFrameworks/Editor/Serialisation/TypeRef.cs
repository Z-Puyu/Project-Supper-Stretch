using System;
using System.Collections.Generic;
using CommonFrameworks.Editor.Utils;
using UnityEngine;

namespace CommonFrameworks.Editor.Serialisation {
    [Serializable]
    public class TypeRef<T> : ISerializationCallbackReceiver, IAlias<Type> {
        [field: SerializeField] private string AssemblyQualifiedName { get; set; } = string.Empty;
        public Type Type { get; private set; } = typeof(void);

        public IEnumerable<Type> Options => typeof(T).GetSubtypes();

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            this.AssemblyQualifiedName = this.Type.AssemblyQualifiedName ?? string.Empty;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if (string.IsNullOrEmpty(this.AssemblyQualifiedName)) {
                return;
            }
            
            Type? type = Type.GetType(this.AssemblyQualifiedName);
            if (type is null) {
                Debug.LogError($"Failed to deserialize type {this.AssemblyQualifiedName}");
            } else {
                this.Type = type;
            }
        }
    }
}
