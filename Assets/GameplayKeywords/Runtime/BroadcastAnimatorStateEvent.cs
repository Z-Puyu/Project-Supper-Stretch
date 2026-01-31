using CommonFrameworks.Events;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayKeywords {
    internal sealed class BroadcastAnimatorStateEvent : AnimatorStateBehaviour {
        [field: SerializeField, TreeDropdown(nameof(this.AllEventKeywords))] 
        private string Event { get; set; } = string.Empty;
        
        private AdvancedDropdownList<string> AllEventKeywords => KeywordUtils.Fetch<EventKeywordSheet>();
        
        protected override void Execute(Animator animator, AnimatorStateInfo state, int layer) {
            animator.Send(new GameMessage(this.Event));
        }
    }
}
