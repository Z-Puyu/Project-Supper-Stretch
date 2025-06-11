using System;

namespace Project.Scripts.AttributeSystem.Attributes;

public interface IAttributeReader {
    public abstract Attribute Read(Enum attribute);
    
    public abstract int ReadCurrent(Enum attribute);
    
    public abstract int ReadBase(Enum attribute);
    
    public abstract int ReadMax(Enum attribute);
}
