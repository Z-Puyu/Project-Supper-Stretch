using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public readonly record struct Modifier(ModifierKey Key, float Value, int Duration = 0)
        : IPresentable<ModifierLocalisationMapping>, IPresentable {
    public ModifierType Type => this.Key.Type;
    public string Target => this.Key.Target;

    private string ValueText => this.Type switch {
        ModifierType.BaseOffset => $"{this.Value:+#;-#;+#}",
        ModifierType.Multiplier => $"{this.Value:+#;-#;+#}%",
        ModifierType.FinalOffset => this.Duration > 0
                ? $"{Mathf.CeilToInt(this.Value / this.Duration):+#;-#;+#}"
                : $"{this.Value:+#;-#;+#}",
        var _ => throw new ArgumentOutOfRangeException()
    };

    private string TargetName => this.Target.Definition<AttributeDefinition, AttributeType>()?.ToString() ??
                                 this.Target.Split('.').Last();

    public Vector3 VectorForm => this.Type switch {
        ModifierType.BaseOffset => new Vector3(this.Value, 0, 0),
        ModifierType.Multiplier => new Vector3(0, this.Value, 0),
        ModifierType.FinalOffset => new Vector3(0, 0, this.Value),
        var _ => Vector3.zero
    };

    public static Modifier From(ModifierData data) => new Modifier(data.Key, data.Value, data.Duration);

    public static Modifier Once(float value, string target, ModifierType type) =>
            new Modifier(new ModifierKey(target, type), value);

    public static Modifier Of(float value, string target, ModifierType type, int duration) =>
            new Modifier(new ModifierKey(target, type), value, duration);

    public static IEnumerable<Modifier> Parse(Vector3 code, string target) {
        if (code == Vector3.zero) {
            return [];
        }

        List<Modifier> modifiers = [];
        if (code.x != 0) {
            modifiers.Add(Modifier.Once(code.x, target, ModifierType.BaseOffset));
        }

        if (code.y != 0) {
            modifiers.Add(Modifier.Once(code.y, target, ModifierType.Multiplier));
        }

        if (code.z != 0) {
            modifiers.Add(Modifier.Once(code.z, target, ModifierType.FinalOffset));
        }

        return modifiers;
    }

    public IEnumerable<Modifier> Reduce(Dictionary<string, AttributeType> definitions) {
        if (!definitions.TryGetValue(this.Target, out AttributeType? type)) {
            return [];
        }

        if (type.IsLeaf) {
            return this.Duration switch {
                0 => [this],
                > 0 => [this with { Value = this.Value / this.Duration, Duration = 0 }],
                < 0 => [this with { Duration = 0 }]
            };
        }

        Modifier self = this;
        return type.Children
                   .Cast<AttributeType>()
                   .Select(t => self with { Key = self.Key with { Target = t } })
                   .SelectMany(m => m.Reduce(definitions));
    }

    public string FormatAsText(ModifierLocalisationMapping reference) {
        string desc = !reference
                ? $"{this.ValueText} {this.TargetName}"
                : reference.Map(this).Replace("{value}", this.ValueText);
        return this.Duration switch {
            > 0 => this.Type == ModifierType.FinalOffset 
                    ? $"{desc} per second, for {this.Duration} seconds" 
                    : $"{desc} for {this.Duration} seconds",
            < 0 when this.Type != ModifierType.FinalOffset => $"{desc} per second",
            var _ => desc
        };
    }
    
    public string FormatAsText() {
        return this.Duration switch {
            > 0 => this.Type == ModifierType.FinalOffset 
                    ? $"{this.ValueText} {this.TargetName} per second, for {this.Duration} seconds" 
                    : $"{this.ValueText} {this.TargetName} for {this.Duration} seconds",
            < 0 when this.Type != ModifierType.FinalOffset => $"{this.ValueText} {this.TargetName} per second",
            var _ => $"{this.ValueText} {this.TargetName}"
        };
    }

    public override string ToString() {
        return $"{this.Type}: {this.ValueText} {this.TargetName} for {this.Duration} seconds";
    }

    public static Modifier operator -(Modifier m) {
        return m with { Value = -m.Value };
    }

    public static Modifier operator *(Modifier m, float coefficient) {
        return m with { Value = m.Value * coefficient };
    }

    public static Modifier operator *(Modifier m, int coefficient) {
        return m with { Value = m.Value * coefficient };
    }

    public static Modifier operator +(Modifier m, Modifier other) {
        if (m.Key != other.Key || m.Duration != other.Duration) {
            throw new ArgumentException("Cannot add modifiers with different keys or durations");
        }

        return m with { Value = m.Value + other.Value };
    }

    public static Modifier operator -(Modifier m, Modifier other) {
        if (m.Key != other.Key || m.Duration != other.Duration) {
            throw new ArgumentException("Cannot add modifiers with different keys or durations");
        }

        return m with { Value = m.Value - other.Value };
    }

    public static bool operator ==(Modifier m, int value) {
        return Mathf.Approximately(m.Value, value);
    }

    public static bool operator !=(Modifier m, int value) {
        return !(m == value);
    }

    public static implicit operator Vector3(Modifier m) {
        return m.VectorForm;
    }
}
