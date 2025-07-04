using System.Linq;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Common.GameplayTags;

[CreateAssetMenu(fileName = "Gameplay Tag Definition", menuName = "Gameplay Tags/Gameplay Tag Definition")]
public class GameplayTagDefinition : GameplayTagTree<GameplayTag> {
    [AdvancedDropdown(nameof(this.Leaves))]
    public string testLeaves = string.Empty;
    
    [AdvancedDropdown(nameof(this.All))]
    public string testAll = string.Empty;
    
    private AdvancedDropdownList<string> Leaves => GameplayTagTree<GameplayTag>.Instances
                                                                               .OfType<GameplayTagDefinition>()
                                                                               .LeafTags();

    private AdvancedDropdownList<string> All => GameplayTagTree<GameplayTag>.Instances
                                                                            .OfType<GameplayTagDefinition>()
                                                                            .AllTags();
}
