using Project.Scripts.AttributeSystem.Attributes.Definitions;
using SaintsField;

namespace Project.Scripts.AttributeSystem.Attributes;

public interface IAttributeReader {
    public abstract Attribute Read(string attribute);
    
    public abstract int ReadCurrent(string attribute);
    
    public abstract int ReadBase(string attribute);
    
    public abstract int ReadMax(string attribute);

    public abstract AdvancedDropdownList<string> AllAccessibleAttributes { get; }
}
