using System;
using System.Text;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;
using Project.Scripts.Util.Builder;
using Project.Scripts.Util.Visitor;
using SaintsField;
using UnityEngine;
using Attribute = Codice.Client.BaseCommands.Attribute;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public class Modifier : IVisitor<AttributeSet> {
    public Enum Target { get; private init; }
    private protected ModifierType ValueType { get; set; }
    private protected ModifierOperation OperationType { get; set; }
    private float Value { get; set; }

    private Modifier(Enum target, float value = 0) {
        this.Target = target;
        this.Value = value;
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

    public void Visit(AttributeSet attributes) {
        attributes.AddModifier(this);
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
        return new Modifier(m.Target, -m.Value) {
            ValueType = m.ValueType,
            OperationType = m.OperationType
        };
    }
    
    public sealed class Builder : FluentBuilder<Modifier> {
        private Builder(Modifier template) : base(template) { }

        public static Builder Of(float value, Enum attribute) {
            return new Builder(new Modifier(attribute, value));
        }

        public Modifier As(ModifierType type, ModifierOperation op) {
            this.Template.ValueType = type;
            this.Template.OperationType = op;
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
    }
}