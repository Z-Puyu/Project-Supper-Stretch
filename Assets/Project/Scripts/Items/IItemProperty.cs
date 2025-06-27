using Project.Scripts.Common;

namespace Project.Scripts.Items;

public interface IItemProperty : IPresentable;

public interface IItemProperty<in T> : IItemProperty {
    public abstract void Process(in Item item, T target);   
}
