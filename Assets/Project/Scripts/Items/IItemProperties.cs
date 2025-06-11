using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.Items;

public interface IItemProperties {
    public abstract IEnumerable<Modifier> ApplyOn(AttributeSet target);

    public virtual IEnumerable<Modifier> RevokeFrom(AttributeSet target) {
        return this.ApplyOn(target).Select(modifier => -modifier);
    }
}
