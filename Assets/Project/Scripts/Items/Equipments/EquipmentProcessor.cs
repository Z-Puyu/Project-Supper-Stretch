using Project.Scripts.Util.ChainOfResponsibilities;

namespace Project.Scripts.Items.Equipments;

public class EquipmentProcessor : Processor<ItemProcessingArgs> {
    protected override (ProcessorStatus status, ItemProcessingArgs data) RunProcess(ItemProcessingArgs data) {
        if (data.Item.Type != ItemType.Equipment) {
            return (ProcessorStatus.Healthy, data);
        }
        
        data.OnUseEquipment.Invoke(data.Item);
        return (ProcessorStatus.Completed, data);
    }
}
