using System;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Util.Linq;

namespace Project.Scripts.Items;

public sealed record class ConsumableProperty(Modifier[] Effects, GameplayEffect GameplayEffectOnUse)
        : ItemProperty, IItemProperty<AttributeSet>, IItemProperty<Inventory> {
    private ConsumableProperty(ConsumableProperty property) : base(property) {
        this.Effects = property.Effects;
        this.GameplayEffectOnUse = property.GameplayEffectOnUse;
    }
    
    public void Process(in Item item, AttributeSet target) {
        GameplayEffectExecutionArgs args =
                GameplayEffectExecutionArgs.Builder.From(target).WithCustomModifiers(this.Effects).Build();
        target.AddEffect(this.GameplayEffectOnUse, args);
    }

    public void Process(in Item item, Inventory target) {
        target.Remove(item);
    }

    public override string FormatAsText(ModifierLocalisationMapping mapping) {
        return string.Join('\n', this.Effects.Select(effect => effect.FormatAsText(mapping)));
    }

    public override string FormatAsText() {
        return string.Join('\n', this.Effects.Select(effect => effect.FormatAsText()));       
    }

    public bool Equals(ConsumableProperty? other) {
        return other is not null && this.Effects.SequenceEqual(other.Effects) &&
               this.GameplayEffectOnUse == other.GameplayEffectOnUse;
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        this.Effects.ForEach(hash.Add);
        return HashCode.Combine(hash.ToHashCode(), this.GameplayEffectOnUse);
    }
}
