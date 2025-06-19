using System;
using Editor;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public struct ModifierKey {
#if UNITY_EDITOR
    public static AdvancedDropdownList<ModifierKey> AllKeys =>
            ObjectCache<AttributeDefinition>.Instance.Objects.FetchAllModifiers();
#endif
    [field: SerializeField] private string TargetAttribute { get; set; }
    [field: SerializeField] private string Type { get; set; }
    [field: SerializeField] private string Operation { get; set; }

    public static ModifierKey Of(AttributeKey attribute, ModifierType type, ModifierOperation op) {
        return (type, op) switch {
            (ModifierType.Base, ModifierOperation.Offset) => new ModifierKey
                    { TargetAttribute = attribute.Name, Type = "Base", Operation = "Offset" },
            (ModifierType.Current, ModifierOperation.Offset) => new ModifierKey
                    { TargetAttribute = attribute.Name, Type = "Current", Operation = "Offset" },
            (ModifierType.Base, ModifierOperation.Multiplier) => new ModifierKey
                    { TargetAttribute = attribute.Name, Type = "Base", Operation = "Multiplier" },
            (ModifierType.Current, ModifierOperation.Multiplier) => new ModifierKey
                    { TargetAttribute = attribute.Name, Type = "Current", Operation = "Multiplier" },
            var _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public override string ToString() {
        return $"{this.Type} {this.TargetAttribute} {this.Operation}";
    }

    public static bool operator ==(ModifierKey key, string @string) {
        return key.ToString() == @string;
    }

    public static bool operator !=(ModifierKey key, string @string) {
        return !(key == @string);
    }
}
