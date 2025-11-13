using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Attributes;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public class EffectSource {
        public IAttributeReader Instigator { get; }
        private readonly Dictionary<string, double> userData = new Dictionary<string, double>();
        public IReadOnlyDictionary<string, double> UserData => this.userData; 
        
        internal EffectSource(IAttributeReader instigator) {
            this.Instigator = instigator;
        }

        public EffectSource WithUserData(string key, double value) {
            this.userData[key] = value;
            return this;
        }

        public EffectSource WithoutUserData(string key) {
            this.userData.Remove(key);
            return this;
        }
    }
}
