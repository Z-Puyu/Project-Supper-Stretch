using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Editor;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Util.Builder;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public record class Modifier {
    public ModifierKey Key { get; private set; }
    
    [field: SerializeField, OnValueChanged(nameof(this.GenerateKey)), AdvancedDropdown(nameof(this.AllTargets))] 
    public string Target { get; set; } = string.Empty;
    
    [field: SerializeField, OnValueChanged(nameof(this.GenerateKey))] 
    public ModifierType Method { get; set; }
    
    [field: SerializeField, OnValueChanged(nameof(this.GenerateKey))] 
    public float Value { get; set; }
    
    public Vector4 VectorForm => this.Method switch {
        ModifierType.BaseOffset => new Vector4(this.Value, 0, 0, 0),
        ModifierType.Multiplier => new Vector4(0, this.Value, 0, 0),
        ModifierType.FinalOffset => new Vector4(0, 0, this.Value, 0),
        ModifierType.ModifierScaling => new Vector4(0, 0, 0, this.Value),
        var _ => throw new ArgumentOutOfRangeException()
    };
    
    private AdvancedDropdownList<string> AllTargets => ObjectCache<AttributeDefinition>.Instance.Objects.AllTags();

    public static Modifier Of(float value, string target, ModifierType type) {
        if (type is ModifierType.Multiplier or ModifierType.ModifierScaling) {
            value = Mathf.Max(value, -100);       
        }
        
        return new Modifier { Key = new ModifierKey(target, type), Value = value, Target = target, Method = type };
    }
    
    public static IEnumerable<Modifier> Parse(Vector4 code, string target) {
        if (code == Vector4.zero) {
            return [];
        }
        
        List<Modifier> modifiers = [];
        if (code.x != 0) {
            modifiers.Add(Modifier.Of(code.x * code.w, target, ModifierType.BaseOffset));
        }
        
        if (code.y != 0) {
            modifiers.Add(Modifier.Of(code.y * code.w, target, ModifierType.Multiplier));
        }
        
        if (code.z != 0) {
            modifiers.Add(Modifier.Of(code.z, target, ModifierType.FinalOffset));
        }
        
        return modifiers;
    }
    
    private void GenerateKey() {
        this.Key = new ModifierKey(this.Target, this.Method);
    }

    public IEnumerable<Modifier> Reduce(Dictionary<string, AttributeType> definitions) {
        if (!definitions.TryGetValue(this.Target, out AttributeType? type)) {
            return [];
        }

        if (type.IsLeaf) {
            return [this];
        }

        return type.Children
                   .Cast<AttributeType>()
                   .Select(t => this with { Target = t })
                   .SelectMany(m => m.Reduce(definitions));
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.Value.ToString("+#;-#;+#"));
        if (this.Method == ModifierType.Multiplier) {
            sb.Append('%');
        }

        return sb.Append(this.Method switch {
            ModifierType.BaseOffset => " base ",
            var _ => " "
        }).Append(this.Target.Split('.')[^1]).ToString();
    }

    public static Modifier operator -(Modifier m) {
        return Modifier.Of(-m.Value, m.Target, m.Method);
    }

    public static Modifier operator *(Modifier m, float coefficient) {
        return Modifier.Of(m.Value * coefficient, m.Target, m.Method);
    }
    
    public static Modifier operator *(Modifier m, int coefficient) {
        return Modifier.Of(m.Value * coefficient, m.Target, m.Method);
    }

    public static Modifier operator +(Modifier m, Modifier other) {
        if (m.Target != other.Target || m.Method != other.Method) {
            throw new ArgumentException("Cannot add modifiers with different keys");
        }
        
        return Modifier.Of(m.Value + other.Value, m.Target, m.Method);
    }
    
    public static Modifier operator -(Modifier m, Modifier other) {
        if (m.Key != other.Key) {
            throw new ArgumentException("Cannot add modifiers with different keys");
        }
        
        return Modifier.Of(m.Value - other.Value, m.Target, m.Method);
    }

    public static implicit operator Vector4(Modifier m) {
        return m.VectorForm;
    }
}