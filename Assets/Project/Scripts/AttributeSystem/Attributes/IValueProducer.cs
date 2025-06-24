namespace Project.Scripts.AttributeSystem.Attributes;

public interface IValueProducer {
    public abstract int ProduceFrom(IAttributeReader target, IAttributeReader source);
}
