using System;
using Project.Scripts.Items.Equipments;

namespace Project.Scripts.Items;

public record class ItemProcessingArgs(Item Item, Action<Item> OnConsume, Action<Item> OnUseEquipment);
