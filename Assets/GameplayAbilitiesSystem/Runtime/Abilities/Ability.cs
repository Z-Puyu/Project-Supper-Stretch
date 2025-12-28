using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities;

[CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
public sealed class Ability : ScriptableObject {
    [field: SerializeReference, ReferencePicker] 
    private List<AbilityAction> Actions { get; set; } = new List<AbilityAction>();
}
