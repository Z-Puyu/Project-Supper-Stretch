using System;
using System.Collections.Generic;
using Project.Scripts.Util.Builder;
using Project.Scripts.Util.Visitor;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public abstract class Modifier : IVisitor<ModifierManager> {
    [field: SerializeField] 
    protected ModifierType ValueType { get; set; } = ModifierType.Base;
    
    [field: SerializeField] 
    protected ModifierOperation OperationType { get; private set; } = ModifierOperation.Offset;
    
    [field: SerializeField, Header("Magnitude")]
    protected Magnitude? Magnitude { get; set; }
    
    public abstract void Visit(ModifierManager visitable);
    
    public sealed class Configurator : FluentBuilder<Modifier> {
        private Configurator(Modifier template) : base(template) { }
        
        public static Configurator Of(Modifier template) => new Configurator(template);

        public Configurator BasedOn(Attributes.AttributeManagementSystem? self, Attributes.AttributeManagementSystem target) {
            this.Template.Magnitude = this.Template.Magnitude?.BasedOn(self, target);
            return this;
        }
        
        public Configurator With(string label, int magnitude) {
            this.Template.Magnitude = this.Template.Magnitude?.With(label, magnitude);
            return this;
        }

        public Configurator AccordingTo(IReadOnlyDictionary<string, int> magnitudes) {
            this.Template.Magnitude = this.Template.Magnitude?.With(magnitudes);
            return this;
        }
    }
}

/// <summary>
/// Base class for a modifier.
/// </summary>
/// <typeparam name="K">The enum type representing the attribute modifiable by the modifier.</typeparam>
[Serializable]
public abstract class Modifier<K> : Modifier where K : Enum {
    [field: SerializeField] 
    public K Target { get; protected set; }
    
    protected virtual float Value => this.Magnitude?.Evaluate() ?? 0;
    
    protected Modifier(K target) {
        this.Target = target;
    }

    public override void Visit(ModifierManager manager) {
        manager.AddModifier(this);
    }

    public virtual Vector4 ToVector4() {
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
}