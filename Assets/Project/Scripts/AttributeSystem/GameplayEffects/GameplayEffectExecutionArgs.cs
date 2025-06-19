using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Util.Builder;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public class GameplayEffectExecutionArgs {
    public static GameplayEffectExecutionArgs Empty { get; } = new GameplayEffectExecutionArgs();
    
    public IAttributeReader? Instigator { get; private set; }
    public int Chance { get; private set; } = 100;
    public bool HasLevel { get; private set; }
    public int Level { get; private set; } = 1;
    public Func<int, float>? LevelCoefficient { get; private set; }
    public Dictionary<ModifierKey, Modifier> ModifierOverrides { get; private init; } = []; 
    private Dictionary<string, float> UserNumericalData { get; init; } = [];
    private Dictionary<Type, int> UserFlags { get; init; } = [];
    
    public IReadOnlyDictionary<string, float> NumericalData => this.UserNumericalData;
    
    private GameplayEffectExecutionArgs() { }

    public bool HasFlag<E>(E flag) where E : struct, Enum {
        if (!typeof(E).IsDefined(typeof(FlagsAttribute), false)) {
            return false;
        }
        
        int bit = (int)(object)flag;
        return this.UserFlags.TryGetValue(typeof(E), out int flags) && (flags & bit) == bit;
    }

    public sealed class Builder : FluentBuilder<GameplayEffectExecutionArgs> {
        public Builder() : base(new GameplayEffectExecutionArgs()) { }

        public static Builder From(IAttributeReader instigator) {
            Builder builder = new Builder();
            builder.Template.Instigator = instigator;
            return builder;
        }

        public Builder WithChance(int chance) {
            this.Template.Chance = chance;
            return this;       
        }

        public Builder WithNumericalData(string key, float value) {
            this.Template.UserNumericalData[key] = value;
            return this;      
        }

        public Builder WithCustomModifiers(IEnumerable<Modifier> modifiers) {
            foreach (Modifier modifier in modifiers) {
                this.Template.ModifierOverrides[modifier.Key] = modifier;
            }
            
            return this;      
        }
        
        public Builder WithCustomModifier(Modifier modifier) {
            this.Template.ModifierOverrides[modifier.Key] = modifier;
            return this;      
        }

        public Builder WithFlag<E>(E flag) where E : struct, Enum {
            if (!typeof(E).IsDefined(typeof(FlagsAttribute), false)) {
                throw new ArgumentException($"{typeof(E)} is not a flags enum.");
            }

            int bit = (int)(object)flag;
            if (!this.Template.UserFlags.TryAdd(typeof(E), bit)) {
                this.Template.UserFlags[typeof(E)] |= bit;
            }
            
            return this;
        }

        public Builder OfLevel(int level, Func<int, float>? mapping = null) {
            this.Template.HasLevel = true;
            this.Template.Level = level;
            this.Template.LevelCoefficient = mapping;
            return this;
        }
    }
}
