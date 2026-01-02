using System;
using System.Collections.Generic;
using CommonFrameworks.Logic;
using GameplayAbilitiesSystem.Runtime.Effects;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Predicate {
    [Serializable]
    public struct HasKeywords : IPredicate<IEffectEmitterFacade>, IPredicate<IEffectReceiverFacade> {
        private enum PredicateType {
            All,
            Any,
            None,
            [LabelText("Not All")] NotAll,
            [LabelText("Exactly One")] ExactlyOne
        }

        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> Keywords { get; set; } = new List<string>();

        [field: SerializeField] private PredicateType Predicate { get; set; } = PredicateType.All;
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList();
        
        public HasKeywords() { }
        
        public bool Holds(IEffectEmitterFacade source) {
            switch (this.Predicate) {
                case PredicateType.All:
                    foreach (string keyword in this.Keywords) {
                        if (!source.EmitterKeywordContainer.Contains(keyword)) {
                            return false;
                        }
                    }
                    
                    return true;
                case PredicateType.Any:
                    foreach (string keyword in this.Keywords) {
                        if (source.EmitterKeywordContainer.Contains(keyword)) {
                            return true;
                        }
                    }
                    
                    return false;
                case PredicateType.None:
                    foreach (string keyword in this.Keywords) {
                        if (source.EmitterKeywordContainer.Contains(keyword)) {
                            return false;
                        }
                    }
                    
                    return true;
                case PredicateType.NotAll:
                    foreach (string keyword in this.Keywords) {
                        if (!source.EmitterKeywordContainer.Contains(keyword)) {
                            return true;
                        }
                    }
                    
                    return false;
                case PredicateType.ExactlyOne:
                    int count = 0;
                    foreach (string keyword in this.Keywords) {
                        if (source.EmitterKeywordContainer.Contains(keyword)) {
                            count += 1;
                        }
                        
                        if (count > 1) {
                            return false;
                        }
                    }

                    return count == 1;
                default:
                    return true;
            }
        }
        
        public bool Holds(IEffectReceiverFacade receiver) {
            switch (this.Predicate) {
                case PredicateType.All:
                    foreach (string keyword in this.Keywords) {
                        if (!receiver.ReceiverKeywordContainer.Contains(keyword)) {
                            return false;
                        }
                    }
                    
                    return true;
                case PredicateType.Any:
                    foreach (string keyword in this.Keywords) {
                        if (receiver.ReceiverKeywordContainer.Contains(keyword)) {
                            return true;
                        }
                    }
                    
                    return false;
                case PredicateType.None:
                    foreach (string keyword in this.Keywords) {
                        if (receiver.ReceiverKeywordContainer.Contains(keyword)) {
                            return false;
                        }
                    }
                    
                    return true;
                case PredicateType.NotAll:
                    foreach (string keyword in this.Keywords) {
                        if (!receiver.ReceiverKeywordContainer.Contains(keyword)) {
                            return true;
                        }
                    }
                    
                    return false;
                case PredicateType.ExactlyOne:
                    int count = 0;
                    foreach (string keyword in this.Keywords) {
                        if (receiver.ReceiverKeywordContainer.Contains(keyword)) {
                            count += 1;
                        }
                        
                        if (count > 1) {
                            return false;
                        }
                    }

                    return count == 1;
                default:
                    return true;
            }
        }
    }
}
