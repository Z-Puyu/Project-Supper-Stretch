using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    [Serializable]
    internal sealed class SimpleMover : Mover {
        [NotNull] 
        [field: SerializeField, Required] 
        private Transform? RootTransform { get; set; }
        
        public override void MoveBy(Vector3 displacement) {
            this.RootTransform.Translate(displacement, Space.World);
        }
    }
}
