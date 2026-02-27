using System;
using UnityEngine;

namespace GameplayAbilities.EditorTooling {
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SubtypeSelectorAttribute : PropertyAttribute {
        public string PredicateName { get; set; }

        public SubtypeSelectorAttribute(string predicate = "") {
            this.PredicateName = predicate;
        }
    }
}
