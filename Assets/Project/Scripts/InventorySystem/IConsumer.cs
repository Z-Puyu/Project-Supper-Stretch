using Project.Scripts.Items;

namespace Project.Scripts.InventorySystem;

public interface IConsumer {
    public abstract void Consume(in Inventory from, in Item item, int amount = 1);
}