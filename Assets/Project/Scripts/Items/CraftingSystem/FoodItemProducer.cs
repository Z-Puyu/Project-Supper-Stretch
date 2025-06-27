using System;
using System.Collections.Generic;
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

        List<Modifier> transformed = [];
        foreach (Modifier modifier in modifiers) {
            AttributeType? type = modifier.Target.Definition<AttributeDefinition, AttributeType>();
            if (type is null) {
                throw new ArgumentException($"{modifier.Target} is not an attribute");       
            }

            switch (modifier.Type) {
                case ModifierType.Multiplier:
                    transformed.Add(modifier);
                    break;
                case ModifierType.BaseOffset:
                    transformed.Add(type.BehaveLikeHealth
                            ? modifier with { Key = modifier.Key with { Type = ModifierType.FinalOffset } }
                            : modifier);
                    break;
                default:
                    throw new ArgumentException($"{modifier.Type} is not a valid modifier type");
            }
        }

        return recipe.Cook(this.ItemDefinition)
                     .WithProperty(new ConsumableProperty(transformed.ToArray(), this.EffectOnConsume));
    }
}
