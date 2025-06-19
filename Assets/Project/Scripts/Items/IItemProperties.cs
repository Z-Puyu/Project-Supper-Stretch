using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.Items;

public interface IItemProperties {
    public abstract IEnumerable<Modifier> ApplyOn(AttributeSet target);
}
