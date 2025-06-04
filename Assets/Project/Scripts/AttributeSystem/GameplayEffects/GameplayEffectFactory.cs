using System;
using System.Collections.Generic;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public class GameplayEffectFactory : Singleton<GameplayEffectFactory> {
    [field: SerializeReference]
    private List<IGameplayEffect> GameplayEffects { get; set; } = [];

    private Dictionary<Type, IGameplayEffect> InstantEffects { get; init; } = [];

    protected override void Awake() {
        base.Awake();
        this.GameplayEffects.ForEach(effect => this.InstantEffects.Add(effect.GetType(), effect));
    }
    
    public static IGameplayEffect CreateInstant<T>() where T : IGameplayEffect {
        if (Singleton<GameplayEffectFactory>.Instance.InstantEffects.TryGetValue(typeof(T), out IGameplayEffect e)) {
            return e;
        }
        
        throw new ArgumentException($"No instant gameplay effect of type {typeof(T)} found.");
    }
}
