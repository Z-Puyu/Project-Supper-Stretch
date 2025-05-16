using Project.Scripts.Events;
using Project.Scripts.InventorySystem.Items;
using UnityEngine;

namespace Project.Scripts.InventorySystem;

[CreateAssetMenu(fileName = "Inventory Entry Event Channel", menuName = "Event System/Inventory Entry Event Channel")]
public class InventoryEntryEventChannel : EventChannel<(Item item, int count)>;