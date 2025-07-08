using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Items.Definitions;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items;

[CreateAssetMenu(fileName = "Item Data", menuName = "Item/Data")]
public class ItemData : ScriptableObject {
    [field: SerializeField, AdvancedDropdown(nameof(this.AllItemTypes))]
    public string Type { get; private set; } = string.Empty;

    [field: SerializeField] public string Name { get; private set; } = string.Empty;
    [field: SerializeField, MinValue(0)] public int Worth { get; private set; } = 1;
    
    [field: SerializeReference, RichLabel(nameof(this.Label), true)] 
    public List<IItemPropertyData> ItemProperties { get; private set; } = [];

    private AdvancedDropdownList<string> AllItemTypes {
        get {
            if (GameplayTagTree<ItemType>.Instances.Count == 0) {
                Resources.LoadAll<ItemDefinition>("");
            }

            return GameplayTagTree<ItemType>.Instances
                                            .OfType<ItemDefinition>()
                                            .AllTags();
        }
    }

    private string Label(object obj, int _) => obj.GetType().Name;
}