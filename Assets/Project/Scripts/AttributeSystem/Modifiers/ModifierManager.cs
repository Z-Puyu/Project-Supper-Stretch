using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Common;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public class ModifierManager {
    private Dictionary<string, Vector4> Modifiers { get; init; } = [];

    public IEnumerable<Modifier> NetModifiers =>
            this.Modifiers.SelectMany(pair => Modifier.Parse(pair.Value, pair.Key));
    
    public float Query(string key, int @base) {
        return this.Modifiers.TryGetValue(key, out Vector4 modifier)
                ? (@base + modifier.x * modifier.w) * (100 + modifier.y + modifier.w) / 100.0f + modifier.z
                : @base;
    }

    public void Add(Modifier modifier) {
        if (modifier.Key.TargetAttribute == string.Empty) {
            Debug.LogError("Modifier must have a target attribute");
            return;
        }
        
        this.Modifiers.TryAdd(modifier.Target, Vector4.zero);
        this.Modifiers[modifier.Target] += modifier.VectorForm;
    }

    public void Remove(Modifier modifier) {
        if (modifier.Key.TargetAttribute == string.Empty) {
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
