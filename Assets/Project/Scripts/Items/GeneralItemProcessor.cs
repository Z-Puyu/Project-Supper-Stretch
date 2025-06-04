using Project.Scripts.Util.ChainOfResponsibilities;

namespace Project.Scripts.Items;

public class GeneralItemProcessor : Processor<ItemProcessingArgs> {
    protected override (ProcessorStatus status, ItemProcessingArgs data) RunProcess(ItemProcessingArgs data) {
        data.OnConsume.Invoke(data.Item);
        return (ProcessorStatus.Healthy, data);
    }
}
