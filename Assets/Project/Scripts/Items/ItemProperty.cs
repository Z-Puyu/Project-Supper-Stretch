namespace Project.Scripts.Items;

public abstract record class ItemProperty : IItemProperty {
    protected ItemProperty(ItemProperty property) { }
    
    public abstract string FormatAsText();
}
