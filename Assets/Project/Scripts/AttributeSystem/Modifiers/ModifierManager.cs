using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public class ModifierManager {
    [Flags]
    public enum QueryFlag {
        None = 0,
        NonNegativeBase = 1 << 0,
        NoMultiplierOnNegativeBase = 1 << 1,
        FlipMultiplierSignOnNegativeBase = 1 << 2,
        AllowNegativeResult = 1 << 3
    }

    private Dictionary<string, Vector3> Modifiers { get; init; } = [];
    
    public IEnumerable<string> AffectedAttributes => this.Modifiers.Keys;

    public float ModifierMagnitude(ModifierType type, string key) {
        return type switch {
            ModifierType.BaseOffset => this.Modifiers.TryGetValue(key, out Vector3 modifier) ? modifier.x : 0,
            ModifierType.Multiplier => this.Modifiers.TryGetValue(key, out Vector3 modifier) ? modifier.y : 0,
            ModifierType.FinalOffset => this.Modifiers.TryGetValue(key, out Vector3 modifier) ? modifier.z : 0,
            var _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static float Calculate(Vector3 modifier, int @base, QueryFlag flags) {
        float value = @base + modifier.x;
        if (flags.HasFlag(QueryFlag.NonNegativeBase)) {
            value = Mathf.Max(value, 0);
        }

        if (Mathf.Approximately(value, 0)) {
            value += modifier.z;
            return Mathf.Approximately(value, 0) ? 0 : value;       
        }

        if (flags.HasFlag(QueryFlag.NoMultiplierOnNegativeBase)) {
            value = value * (100 + Mathf.Max(0, Mathf.Sign(value)) * modifier.y) / 100 + modifier.z;
        } else if (flags.HasFlag(QueryFlag.FlipMultiplierSignOnNegativeBase)) {
            value = value * (100 + Mathf.Sign(value) * modifier.y) / 100 + modifier.z;
        } else {
            value = value * (100 + modifier.y) / 100 + modifier.z;       
        }

        if (flags.HasFlag(QueryFlag.AllowNegativeResult)) {
            return Mathf.Approximately(value, 0) ? 0 : value;
        }
        
        return Mathf.Approximately(value, 0) ? 0 : Mathf.Max(value, 0);
    }
    
    public float Query(string key, int @base, QueryFlag flags = QueryFlag.NonNegativeBase) {
        return this.Modifiers.TryGetValue(key, out Vector3 modifier)
                ? ModifierManager.Calculate(modifier, @base, flags)
                : @base;
    }

    public float Project(string key, Modifier modifier, int @base = 0, QueryFlag flags = QueryFlag.NonNegativeBase) {
        Vector3 projected = this.Modifiers.TryGetValue(key, out Vector3 current)
                ? modifier.VectorForm + current
                : modifier.VectorForm;
        return ModifierManager.Calculate(projected, @base, flags);
    }

    public void Add(Modifier modifier) {
        if (modifier.Target == string.Empty) {
            Debug.LogError("Modifier must have a target attribute");
            return;
        }
        
        Vector3 current = this.Modifiers.GetValueOrDefault(modifier.Target, Vector3.zero);
        Vector3 @new = modifier.VectorForm + current;
        if (@new.y < -100) {
            @new = @new with { y = -100 };
        }
        
        this.Modifiers[modifier.Target] = @new;
    }

    public void Remove(Modifier modifier) {
        if (modifier.Target == string.Empty) {
            throw new ArgumentException("Modifier must have a target attribute");       
        }
        
        if (!this.Modifiers.ContainsKey(modifier.Target)) {
            throw new ArgumentException($"""
                                         No modifier for {modifier.Target}. You probably should add the negative of 
                                         the modifier instead.");
                                         """);
        }
        
        this.Modifiers[modifier.Target] -= modifier.VectorForm;
    }
}
