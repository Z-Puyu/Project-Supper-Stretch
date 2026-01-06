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
            this.Interrupter.Cancel();
            this.Interrupter.Dispose();
            foreach (Keyword keyword in this.GrantedKeywords) {
                system.EmitterKeywordContainer.Remove(keyword);
            }
            
            foreach (Keyword keyword in this.RevokedKeywords) {
                system.EmitterKeywordContainer.Add(keyword);
            }
        }
    }
}
