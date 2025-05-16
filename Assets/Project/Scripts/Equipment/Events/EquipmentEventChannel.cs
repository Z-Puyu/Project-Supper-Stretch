using Project.Scripts.Events;
using UnityEngine;

namespace Project.Scripts.Equipment.Events;

[CreateAssetMenu(fileName = "Equipment Event Channel", menuName = "Event System/Equipment Event Channel")]
public class EquipmentEventChannel : EventChannel<EquipmentChangeArgs>;
