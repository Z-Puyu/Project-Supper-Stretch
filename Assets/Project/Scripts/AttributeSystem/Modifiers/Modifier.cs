using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Util.Visitor;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public abstract class Modifier : IVisitor<ModifierManager> {
    public enum Type { Base, Current }

    public enum Operation { Offset, Multiplier }

    [field: SerializeField] 
    public AttributeType Target { get; private set; }
    
    [field: SerializeField] 
    public Type ValueType { get; private set; }
    
    [field: SerializeField] 
    public Operation OperationType { get; private set; }

    [NotNull]
    [field: SerializeReference, SubclassSelector]
    private Magnitude? Magnitude { get; set; }

    public void ConfigureMagnitude(string label, int magnitude) {
        if (this.Magnitude is SetByCallerMagnitude m && m.Label == label) {
            m.Value = magnitude;
        }
    }

    public void AggregateTo(AttributeSet attributes, ref float baseOffset, ref float baseMultiplier) {
        if (this.ValueType == Type.Current) {
            return;
        }

        switch (this.OperationType) {
            case Operation.Offset:
                baseOffset += this.Magnitude.GetValueWith(attributes);
                break;
            case Operation.Multiplier:
                baseMultiplier += this.Magnitude.GetValueWith(attributes);
                break;
        }
    }

    public Vector2 ToFinalValueModification(AttributeSet attributes) {
        if (this.ValueType == Type.Base) {
            return Vector2.up;
        }
        
        float offset = this.OperationType == Operation.Offset
                ? this.Magnitude.GetValueWith(attributes) 
                : 0;
        float multiplier = this.OperationType == Operation.Multiplier
                ? this.Magnitude.GetValueWith(attributes)
                : 0;
        return new Vector2(offset, multiplier);
    }

    public void Visit(ModifierManager manager) {
        switch (this.ValueType) {
            case Type.Current:
                manager.AddAsFinalValueModifier(this);
                break;
            case Type.Base:
                switch (this.OperationType) {
                    case Operation.Offset:
                        manager.AddAsBaseValueOffset(this);
                        break;
                    case Operation.Multiplier:
                        manager.AddAsBaseValueMultiplier(this);
                        break;
                }
                break;
        }
    }
}