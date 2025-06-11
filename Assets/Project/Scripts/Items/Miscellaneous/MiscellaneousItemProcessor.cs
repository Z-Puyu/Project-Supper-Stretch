using Project.Scripts.Util.ChainOfResponsibilities;

namespace Project.Scripts.Items.Miscellaneous;

public class MiscellaneousItemProcessor : Processor<ItemProcessingArgs> {
    protected override (ProcessorStatus status, ItemProcessingArgs data) RunProcess(ItemProcessingArgs data) {
        return (ProcessorStatus.Failure, data);
    }
}
