using System;

namespace Project.Scripts.AttributeSystem.Attributes;

internal readonly record struct EnumKey(Type Type, Enum Value, string Name) {
    public override string ToString() {
        return $"{this.Type}.{this.Name}";
    }

    internal E As<E>() where E : Enum {
        if (Enum.IsDefined(typeof(E), this.Value)) {
            return (E)this.Value;
        }
        
        throw new InvalidCastException($"Cannot cast {this} to {typeof(E)}");
    }

    internal static EnumKey Of<E>(E @enum) where E : Enum {
        return new EnumKey(typeof(E), @enum, @enum.ToString());
    }

    public static implicit operator Enum(EnumKey key) => key.Value;
    public static implicit operator string(EnumKey key) => key.Name;
}
