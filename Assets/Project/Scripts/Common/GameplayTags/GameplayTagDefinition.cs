using Editor;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Common.GameplayTags;

[CreateAssetMenu(fileName = "Gameplay Tag Definition", menuName = "Gameplay Tags/Gameplay Tag Definition")]
public class GameplayTagDefinition : GameplayTagTree<GameplayTag> {
    [AdvancedDropdown(nameof(this.Leaves))]
    public string testLeaves = string.Empty;
    
    [AdvancedDropdown(nameof(this.All))]
    public string testAll = string.Empty;
    
    private AdvancedDropdownList<string> Leaves => ObjectCache<GameplayTagDefinition>.Instance.Objects.LeafTags();

    private AdvancedDropdownList<string> All => ObjectCache<GameplayTagDefinition>.Instance.Objects.AllTags();
}
