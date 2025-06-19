using System;
using System.Text;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Util.Builder;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public struct Modifier {
    [field: SerializeField, ReadOnly] public ModifierKey Key { get; private set; }
    
    [field: SerializeField, OnValueChanged(nameof(this.GenerateKey))] 
    public AttributeKey Target { get; set; }
    
    [field: SerializeField, OnValueChanged(nameof(this.GenerateKey))] 
    public ModifierType ValueType { get; set; }
    
    [field: SerializeField, OnValueChanged(nameof(this.GenerateKey))] 
    private ModifierOperation OperationType { get; set; }
    
    [field: SerializeField, OnValueChanged(nameof(this.GenerateKey))] 
    public float Value { get; set; }
    
    private void GenerateKey() {
        this.Key = ModifierKey.Of(this.Target, this.ValueType, this.OperationType);
    }
    
    public Vector4 ToVector4() {
        switch (this.ValueType) {
            case ModifierType.Base:
                switch (this.OperationType) {
                    case ModifierOperation.Offset:
                        return new Vector4(this.Value, 0, 0, 0);
                    case ModifierOperation.Multiplier:
                        return new Vector4(0, this.Value, 0, 0);
                }

                break;
            case ModifierType.Current:
                switch (this.OperationType) {
                    case ModifierOperation.Offset:
                        return new Vector4(0, 0, this.Value, 0);
                    case ModifierOperation.Multiplier:
                        return new Vector4(0, 0, 0, this.Value);
                }

                break;
        }

        return Vector4.zero;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.Value < 0 ? '-' : '+').Append(Math.Abs(this.Value));
        if (this.OperationType == ModifierOperation.Multiplier) {
            sb.Append('%');
        }

        return sb.Append(this.ValueType switch {
            ModifierType.Base => " base ",
            var _ => " "
        }).Append(this.Target).ToString();
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

    public static implicit operator Vector4(Modifier m) {
        return m.ToVector4();
    }
    
    public sealed class Builder : FluentBuilder<Modifier> {
        private Builder(Modifier template) : base(template) { }

        public static Builder Of(float value, AttributeKey attribute) {
            return new Builder(new Modifier { Target = attribute, Value = value });
        }

        public Modifier As(ModifierType type, ModifierOperation op) {
            this.Template = this.Template with { ValueType = type };
            this.Template = this.Template with { OperationType = op };
            return this.Build();
        }

        public Modifier BaseOffset() {
            return this.As(ModifierType.Base, ModifierOperation.Offset);
        }
        
        public Modifier FinalOffset() {
            return this.As(ModifierType.Current, ModifierOperation.Offset);
        }
        
        public Modifier BaseMultiplier() {
            return this.As(ModifierType.Base, ModifierOperation.Multiplier);
        }

        public Modifier FinalMultiplier() {
            return this.As(ModifierType.Current, ModifierOperation.Multiplier);
        }

        public override Modifier Build() {
            this.Template = this.Template with {
                Key = ModifierKey.Of(this.Template.Target, this.Template.ValueType, this.Template.OperationType)
            };
            return base.Build();
        }
    }
}