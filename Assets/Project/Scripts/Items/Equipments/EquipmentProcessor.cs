using System.Collections.Generic;
using Project.Scripts.Util.ChainOfResponsibilities;

namespace Project.Scripts.Items.Equipments;

public class EquipmentProcessor : Processor<ItemProcessingArgs> {
    private HashSet<Item> Equipped { get; set; } = [];
    
    protected override (ProcessorStatus status, ItemProcessingArgs data) RunProcess(ItemProcessingArgs data) {
        if (!data.Item.Properties.HasFlag(ItemProperty.Equipable)) {
            return (ProcessorStatus.Healthy, data);
        }
        
        data.OnUseEquipment.Invoke(data.Item);
        return (ProcessorStatus.Completed, data);
    }
}
