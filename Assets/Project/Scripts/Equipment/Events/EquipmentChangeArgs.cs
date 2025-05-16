namespace Project.Scripts.Equipment.Events;

public record class EquipmentChangeArgs(Equipment Equipment, EquipmentSocket Socket, bool IsEquipped);
