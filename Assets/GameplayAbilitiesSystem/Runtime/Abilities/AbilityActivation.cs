using System.Collections.Generic;
using System.Threading;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    internal readonly record struct AbilityActivation(
        ICollection<Keyword> GrantedKeywords,
        ICollection<Keyword> RevokedKeywords,
        CancellationTokenSource Interrupter
    ) {
        internal void Stop(AbilitySystem system) {
            foreach (Keyword keyword in this.GrantedKeywords) {
                system.EmitterKeywordContainer.Remove(keyword);
            }
            
            foreach (Keyword keyword in this.RevokedKeywords) {
                system.EmitterKeywordContainer.Add(keyword);
            }
        }
        
        internal void Interrupt(AbilitySystem system) {
            this.Interrupter.Cancel();
            this.Interrupter.Dispose();
        }
    }
}
