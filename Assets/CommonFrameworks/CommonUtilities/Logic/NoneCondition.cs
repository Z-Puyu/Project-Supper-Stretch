using System;
using System.Collections.Generic;
using CommonFrameworks.CommonUtilities.CommonInterfaces;
using SaintsField;
using UnityEngine;

namespace CommonFrameworks.CommonUtilities.Logic {
    [Serializable]
    public abstract class NoneCondition<T> : IPredicate<T> {
        [field: SerializeField, HideInInspector]
        public string Name { get; private set; } = "None of";
        
        [field: SerializeReference, ReferencePicker] 
        private List<IPredicate<T>> Predicates { get; set; } = new List<IPredicate<T>>();

        public bool Holds(T data) {
            return this.Predicates.Count == 0 || !this.Predicates.Exists(predicate => predicate.Holds(data));
        }
    }
}
