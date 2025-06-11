using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.Items.Food;

public class FoodProperties : IItemProperties {
    public IEnumerable<Modifier> ApplyOn(AttributeSet target) {
        throw new System.NotImplementedException();
    }
}
