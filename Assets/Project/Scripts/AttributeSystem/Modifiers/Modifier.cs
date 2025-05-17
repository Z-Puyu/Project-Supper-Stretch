using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.AttributeTypes;
using Project.Scripts.Util.Builder;
using Project.Scripts.Util.Visitor;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public abstract class Modifier : IVisitor<ModifierManager> {
    public enum Type { Base, Current }

    public enum Operation { Offset, Multiplier }

    [field: SerializeField, Header("Modifier Type")] 
    public AttributeType Target { get; protected set; }
    
    [field: SerializeField] 
    public Type ValueType { get; protected set; }
    
    [field: SerializeField] 
    public Operation OperationType { get; protected set; }
    
    protected Modifier(AttributeType target, Type valueType, Operation operation) {
        this.Target = target;
        this.ValueType = valueType;
        this.OperationType = operation;
    }

    protected virtual Modifier BasedOn(AttributeSet? self, AttributeSet target) {
        return this;
    }
    
    protected virtual Modifier With(IReadOnlyDictionary<string, int> magnitudes) {
        return this;
    }

    protected virtual Modifier With(string label, int magnitude) {
        return this;
    }

    protected abstract float Evaluate();

    public void Visit(ModifierManager manager) {
        manager.AddModifier(this);
    }
    
    public static Vector4 operator +(Vector4 vec, Modifier mod) {
        switch (mod.ValueType) {
            case Type.Base:
                switch (mod.OperationType) {
                    case Operation.Offset:
                        return new Vector4(vec.x + mod.Evaluate(), vec.y, vec.z, vec.w);
                    case Operation.Multiplier:
                        return new Vector4(vec.x, vec.y + mod.Evaluate(), vec.z, vec.w);
                }
                break;
            case Type.Current:
                switch (mod.OperationType) {
                    case Operation.Offset:
                        return new Vector4(vec.x, vec.y, vec.z + mod.Evaluate(), vec.w);
                    case Operation.Multiplier:
                        return new Vector4(vec.x, vec.y, vec.z, vec.w + mod.Evaluate());
                }
                break;
        }

        return vec;
    }
    
    public static Vector4 operator -(Vector4 vec, Modifier mod) {
        switch (mod.ValueType) {
            case Type.Base:
                switch (mod.OperationType) {
                    case Operation.Offset:
                        return new Vector4(vec.x - mod.Evaluate(), vec.y, vec.z, vec.w);
                    case Operation.Multiplier:
                        return new Vector4(vec.x, vec.y - mod.Evaluate(), vec.z, vec.w);
                }
                break;
            case Type.Current:
                switch (mod.OperationType) {
                    case Operation.Offset:
                        return new Vector4(vec.x, vec.y, vec.z - mod.Evaluate(), vec.w);
                    case Operation.Multiplier:
                        return new Vector4(vec.x, vec.y, vec.z, vec.w - mod.Evaluate());
                }
                break;
        }

        return vec;
    }

    public sealed class Configurator : FluentBuilder<Modifier> {
        private Configurator(Modifier template) : base(template) { }
        
        public static Configurator Of(Modifier template) => new Configurator(template);

        public Configurator BasedOn(AttributeSet? self, AttributeSet target) {
            this.Template = this.Template.BasedOn(self, target);
            return this;
        }
        
        public Configurator With(string label, int magnitude) {
            this.Template = this.Template.With(label, magnitude);
            return this;
        }

        public Configurator AccordingTo(IReadOnlyDictionary<string, int> magnitudes) {
            this.Template = this.Template.With(magnitudes);
            return this;
        }
    }
}