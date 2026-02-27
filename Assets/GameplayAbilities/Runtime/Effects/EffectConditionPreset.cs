// using System;
// using System.Collections.Generic;
// using CommonFrameworks.Logic;
// using SaintsField;
// using UnityEngine;
//
// namespace GameplayAbilities.Effects {
//     [Serializable]
//     internal sealed class EffectConditionPreset {
//         [field: SerializeReference, ReferencePicker]
//         private List<IPredicate<IEffectEmitterFacade>> SourceConditions { get; set; } =
//             new List<IPredicate<IEffectEmitterFacade>>();
//
//         [field: SerializeReference, ReferencePicker]
//         private List<IPredicate<IEffectReceiverFacade>> TargetConditions { get; set; } =
//             new List<IPredicate<IEffectReceiverFacade>>();
//
//         private bool IsApplicable(IEffectReceiverFacade target) {
//             if (this.TargetConditions.Count == 0) {
//                 return true;
//             }
//
//             foreach (IPredicate<IEffectReceiverFacade> condition in this.TargetConditions) {
//                 if (!condition.Holds(target)) {
//                     return false;
//                 }
//             }
//             
//             return true;
//         }
//
//         private bool IsApplicable(IEffectEmitterFacade source) {
//             if (this.SourceConditions.Count == 0) {
//                 return true;
//             }
//             
//             foreach (IPredicate<IEffectEmitterFacade> condition in this.SourceConditions) {
//                 if (!condition.Holds(source)) {
//                     return false;
//                 }
//             }
//             
//             return true;
//         }
//
//         internal bool IsApplicable(IEffectEmitterFacade source, IEffectReceiverFacade target) {
//             return this.IsApplicable(target) && this.IsApplicable(source);
//         }
//     }
// }