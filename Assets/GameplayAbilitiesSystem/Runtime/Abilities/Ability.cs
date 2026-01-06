using System;
using System.Collections.Generic;
using System.Threading;
using CommonFrameworks.Logic;
using GameplayAbilitiesSystem.Runtime.Abilities.Executions;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
    public sealed class Ability : ScriptableObject {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        internal List<string> Tags { get; private set; } = new List<string>();
        
        [field: SerializeReference, Tooltip("Conditions on the ability system for this ability to be usable")]
        [field: FieldLabelText(nameof(this.LabelCondition), true)]
        private List<IPredicate<AbilitySystem>> Conditions { get; set; } = new List<IPredicate<AbilitySystem>>();
        
        [field: SerializeReference, ReferencePicker] 
        private List<IAbilityExecutor> ExecutionSteps { get; set; } = new List<IAbilityExecutor>();
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList(true);
        
        private string LabelCondition(object condition) {
            return condition.GetType().Name;
        }

        internal bool TryCommit(AbilitySystem system) {
            foreach (IPredicate<AbilitySystem> condition in this.Conditions) {
                if (!condition.Holds(system)) {
                    return false;
                }
            }
            
            return true;
        }

        internal async void Execute(AbilitySystem system, CancellationToken interrupt) {
            try {
                CancellationToken death = system.destroyCancellationToken;
                using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(interrupt, death);
                for (int i = 0; i < this.ExecutionSteps.Count; i += 1) {
                    try {
                        await this.ExecutionSteps[i].Run(system, this, cts.Token);
                        this.ExecutionSteps[i].Complete();
                    } catch (OperationCanceledException) {
                        system.Stop(this);
                        break;
                    }
                }
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }
}
