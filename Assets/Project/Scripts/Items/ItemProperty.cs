using Project.Scripts.AttributeSystem.Modifiers;

namespace Project.Scripts.Items;

public abstract record class ItemProperty : IItemProperty {
    protected ItemProperty(ItemProperty property) { }
    
    public abstract string FormatAsText(ModifierLocalisationMapping reference);
    public abstract string FormatAsText();
}
