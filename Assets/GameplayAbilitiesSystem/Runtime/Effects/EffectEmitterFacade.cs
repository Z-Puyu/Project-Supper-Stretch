using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayKeywordsSystem.Runtime;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public sealed class EffectEmitterFacade {
        public GameObject Object { get; }
        public IAttributeReader Instigator { get; }
        public ISet<Keyword> Tags { get; }
        private readonly Dictionary<string, double> userData;
        public IReadOnlyDictionary<string, double> UserData => this.userData; 
        
        internal EffectEmitterFacade(GameObject @object, IAttributeReader instigator, ISet<Keyword> tags) {
            this.Object = @object;
            this.Instigator = new ReadOnlyAttributeSet(instigator);
            this.Tags = tags;
            this.userData = new Dictionary<string, double>();
        }

        public EffectEmitterFacade WithUserData(string key, double value) {
            this.userData[key] = value;
            return this;
        }

        public EffectEmitterFacade WithoutUserData(string key) {
            this.userData.Remove(key);
            return this;
        }
    }
}