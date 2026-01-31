using System.Diagnostics.CodeAnalysis;
using GameplayBehaviours.Movement;
using GameplayKeywords;
using SaintsField;
using UnityEngine;

namespace Characters {
    [DisallowMultipleComponent]
    public sealed class KeywordEventHandler : MonoBehaviour {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private string KeywordToBlockMovement { get; set; } = string.Empty;
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private string KeywordToBlockRotation { get; set; } = string.Empty;
        
        [NotNull] 
        [field: SerializeField, Required] 
        private Locomotion? LocomotionComponent { get; set; }

        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<KeywordSheet>();
        
        public void HandleNewlyAddedKeyword(Keyword keyword) {
            if (keyword == this.KeywordToBlockMovement) {
                this.LocomotionComponent.CanMove = false;
            } else if (keyword == this.KeywordToBlockRotation) {
                this.LocomotionComponent.CanRotate = false;
            }    
        }
        
        public void HandleRemovedKeyword(Keyword keyword) {
            if (keyword == this.KeywordToBlockMovement) {
                this.LocomotionComponent.CanMove = true;
            } else if (keyword == this.KeywordToBlockRotation) {
                this.LocomotionComponent.CanRotate = true;
            }    
        }
    }
}
