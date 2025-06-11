using Project.Scripts.Util.ChainOfResponsibilities;

namespace Project.Scripts.Items.Consumables;

public class ConsumableProcessor : Processor<ItemProcessingArgs> {
    protected override (ProcessorStatus status, ItemProcessingArgs data) RunProcess(ItemProcessingArgs data) {
        if (data.Item.GetType() != typeof(ConsumableItemData)) {
            return (ProcessorStatus.Healthy, data);
        }

        data.OnConsume.Invoke(data.Item);
        return (ProcessorStatus.Completed, data);
    }
}
