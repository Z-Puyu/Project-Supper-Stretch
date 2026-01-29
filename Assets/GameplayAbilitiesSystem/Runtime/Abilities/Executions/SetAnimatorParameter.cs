using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonFrameworks.Async;
using SaintsField;
using SaintsField.Playa;
using UnityEditor.Animations;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    public class SetAnimatorParameter : AbilityExecutionStep {
        [field: SerializeField] private AnimatorController? AnimatorController { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.AnimatorController))] 
        private AnimatorControllerParameterType Type { get; set; }
        
        [field: SerializeField, MenuDropdown(nameof(this.Parameters)), ShowIf(nameof(this.AnimatorController))] 
        private int Parameter { get; set; }
        
        [field: SerializeField]
        [field: ShowIf(nameof(this.Type), AnimatorControllerParameterType.Int, nameof(this.AnimatorController))]
        private int IntegerValue { get; set; }
        
        [field: SerializeField]
        [field: ShowIf(nameof(this.Type), AnimatorControllerParameterType.Bool, nameof(this.AnimatorController))]
        private bool BoolValue { get; set; }
        
        [field: SerializeField]
        [field: ShowIf(nameof(this.Type), AnimatorControllerParameterType.Float, nameof(this.AnimatorController))]
        private float FloatValue { get; set; }

        private DropdownList<int> Parameters => this.AnimatorController
                ? new DropdownList<int>(
                    this.AnimatorController.parameters
                        .Where(p => p.type == this.Type)
                        .Select(p => ($"{p.name} ({p.nameHash})", p.nameHash))
                )
                : new DropdownList<int>();
        
        protected override Awaitable Execute(Ability.Context context, CancellationToken interrupt) {
            switch (this.Type) {
                case AnimatorControllerParameterType.Trigger:
                    context.Source.SetAnimatorTrigger(this.Parameter);
                    break;
                case AnimatorControllerParameterType.Int:
                    context.Source.SetAnimatorInt(this.Parameter, this.IntegerValue);
                    break;
                case AnimatorControllerParameterType.Bool:
                    context.Source.SetAnimatorBool(this.Parameter, this.BoolValue);
                    break;
                case AnimatorControllerParameterType.Float:
                    context.Source.SetAnimatorFloat(this.Parameter, this.FloatValue);
                    break;
            }
            
            return AsyncTask.CompletedTask;
        }
    }
}
