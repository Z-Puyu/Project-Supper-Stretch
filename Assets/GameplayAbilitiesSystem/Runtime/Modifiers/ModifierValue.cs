using System;

namespace GameplayAbilitiesSystem.Runtime.Modifiers;

/// <summary>
/// Represents a modification that can be applied to a game attribute.
/// Supports different types of operations (Shift, Multiply, Offset) and can be combined using arithmetic operators.
/// </summary>
public readonly struct ModifierValue : IEquatable<ModifierValue> {
    private double Magnitude { get; }
        
    internal ModifierValue(double magnitude) {
        this.Magnitude = Math.Round(magnitude, 3);
    }

    /// <summary>
    /// Applies the modifier to the given value.
    /// </summary>
    /// <param name="value">The original value to modify.</param>
    /// <param name="operation">The type of operation to perform.</param>
    /// <returns>The modified value after applying this modifier.</returns>
    public double ApplyTo(double value, ModifierType operation) {
        return operation switch {
            ModifierType.Shift or ModifierType.Offset => value + this.Magnitude,
            ModifierType.Multiplier => value * Math.Max(100 + this.Magnitude, 0) / 100.0f,
            ModifierType.Override => this.Magnitude,
            var _ => value
        };
    }
        
    public bool Equals(ModifierValue other) {
        return this.Magnitude.CompareTo(other.Magnitude) == 0;
    }

    public override bool Equals(object obj) {
        return obj is ModifierValue other && this.Equals(other);
    }

    public override int GetHashCode() {
        return this.Magnitude.GetHashCode();
    }
        
    public override string ToString() {
        return $"{this.Magnitude}";
    }
        
    public static ModifierValue operator -(ModifierValue m) {
        return new ModifierValue(-m.Magnitude);
    }
        
    public static ModifierValue operator +(ModifierValue a, ModifierValue b) {
        return new ModifierValue(a.Magnitude + b.Magnitude);
    }

    public static ModifierValue operator -(ModifierValue a, ModifierValue b) {
        return new ModifierValue(a.Magnitude - b.Magnitude);
    }

    public static ModifierValue operator *(ModifierValue a, double k) {
        return new ModifierValue(k * a.Magnitude);
    }

    public static ModifierValue operator *(double k, ModifierValue a) {
        return a * k;
    }
        
    public static ModifierValue operator /(ModifierValue a, double k) {
        return new ModifierValue(a.Magnitude / k);
    }

    public static bool operator ==(ModifierValue a, double n) {
        return Math.Abs(a.Magnitude - n) < 0.001;
    }

    public static bool operator !=(ModifierValue a, double n) {
        return Math.Abs(a.Magnitude - n) >= 0.001;
    }

    public static bool operator >(ModifierValue a, double n) {
        return a.Magnitude - n > 0.001;
    }

    public static bool operator <(ModifierValue a, double n) {
        return a.Magnitude - n < 0.001;
    }

    public static bool operator >=(ModifierValue a, double n) {
        return a == n || a > n;
    }

    public static bool operator <=(ModifierValue a, double n) {
        return a == n || a < n;
    }

    public static implicit operator double(ModifierValue value) {
        return value.Magnitude;
    }

    public static implicit operator ModifierValue(double value) {
        return new ModifierValue(value);
    }
}