using Project.Scripts.Items;

namespace Project.Scripts.InventorySystem;

public interface IConsumer {
    public abstract void Consume(Inventory from, Item item);
}