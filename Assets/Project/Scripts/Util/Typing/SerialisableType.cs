using System;
using UnityEngine;

namespace Project.Scripts.Util.Typing;

[Serializable]
public class SerialisableType : ISerializationCallbackReceiver {
    public Type? Type { get; private set; }

    [SerializeField]
    private string assemblyQualifiedName = string.Empty;
    // [field: SerializeField] private string AssemblyQualifiedName { get; set; } = string.Empty;

    void ISerializationCallbackReceiver.OnBeforeSerialize() {
        /*if (this.Type?.AssemblyQualifiedName is not null) {
            this.AssemblyQualifiedName = this.Type?.AssemblyQualifiedName;
        }*/

        if (this.Type?.AssemblyQualifiedName is not null) {
            this.assemblyQualifiedName = this.Type?.AssemblyQualifiedName ?? string.Empty;
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() {
        if (this.assemblyQualifiedName.TryFindTypeByAssemblyQualifiedName(out Type? type)) {
            this.Type = type;
        } else {
            Debug.LogError($"Type not found: {this.assemblyQualifiedName}");
        }
    }
}
