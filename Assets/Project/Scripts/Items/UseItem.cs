using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.Items;

[Serializable]
public class UseItem : GameplayEffect<ItemUsageExecutionArgs> {
    protected override IEnumerable<Modifier> Run(ItemUsageExecutionArgs args) {
        return args.Item.Properties.SelectMany(props => props.ApplyOn(args.Target));
    }
}
