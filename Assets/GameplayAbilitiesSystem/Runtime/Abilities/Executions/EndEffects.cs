using System;
using System.Threading;
using CommonFrameworks.Extensions;
using GameplayAbilitiesSystem.Runtime.Effects;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    internal sealed class EndEffects : AbilityExecutionStep {
        [field: SerializeField] private Ability? SourceAbility { get; set; }
        [field: SerializeField] private Effect? EffectType { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private string EffectTag { get; set; } = string.Empty;
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList(true);

        protected override Awaitable Execute(AbilitySystem system, Ability ability, CancellationToken interrupt) {
            system.StopEffects(this.SourceAbility, this.EffectType, this.EffectTag);
            return AwaitableExtensions.CompletedTask;
        }
    }
}
