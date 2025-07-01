using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common.GameplayTags;
using UnityEngine;

namespace Project.Scripts.Items.CraftingSystem;

[Serializable]
public class FoodItemProducer : ItemProducer {
    [field: SerializeField] private GameplayEffect? EffectOnConsume { get; set; }

    public override Item Produce(Recipe recipe, IEnumerable<Modifier> modifiers) {
        if (!this.EffectOnConsume) {
            throw new ArgumentException("EffectOnConsume is not set");
        }

        List<Modifier> list = modifiers.ToList();
        Dictionary<AttributeType, Dictionary<int, float>> transformed = [];
        Dictionary<AttributeType, float> scalers = [];
        foreach (Modifier modifier in list.Where(modifier => modifier.Type != ModifierType.Multiplier)) {
            AttributeType? type = modifier.Target.Definition<AttributeDefinition, AttributeType>();
            if (type is null) {
                throw new ArgumentException($"{modifier.Target} is not an attribute");       
            }

            if (transformed.TryGetValue(type, out Dictionary<int, float> record)) {
                if (record.ContainsKey(modifier.Duration)) {
                    record[modifier.Duration] += modifier.Value;
                } else {
                    record.Add(modifier.Duration, modifier.Value);
                }
            } else {
                transformed.Add(type, new Dictionary<int, float> { { modifier.Duration, modifier.Value } });       
            }
        }

        foreach (Modifier modifier in list.Where(modifier => modifier.Type == ModifierType.Multiplier)) {
            AttributeType? type = modifier.Target.Definition<AttributeDefinition, AttributeType>();
            if (type is null) {
                throw new ArgumentException($"{modifier.Target} is not an attribute");       
            }

            if (scalers.TryGetValue(type, out float value)) {
                scalers[type] += modifier.Value;
            } else {
                scalers.Add(type, modifier.Value);       
            }
        }
        
        List<Modifier> transformedModifiers = [];
        foreach (KeyValuePair<AttributeType, Dictionary<int, float>> pair in transformed) {
            foreach (KeyValuePair<int, float> entry in pair.Value) {
                float scaler = Mathf.Max(0, (100 + scalers.GetValueOrDefault(pair.Key, 0)) / 100);
                transformedModifiers.Add(Modifier.Of(entry.Value * scaler, pair.Key.Name,
                    pair.Key.BehaveLikeHealth ? ModifierType.FinalOffset : ModifierType.BaseOffset, entry.Key));
            }
        }
        
        return recipe.Cook(this.ItemDefinition)
                     .WithProperty(new ConsumableProperty(transformedModifiers.ToArray(), this.EffectOnConsume));
    }
}
