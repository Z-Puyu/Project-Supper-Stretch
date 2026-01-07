using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal sealed class EffectRegistry {
        private ConcurrentDictionary<EffectDescriptor, List<CancellationTokenSource>> ActiveEffects { get; } =
            new ConcurrentDictionary<EffectDescriptor, List<CancellationTokenSource>>();
        
        private readonly ReaderWriterLockSlim mutex = new ReaderWriterLockSlim();

        private void Add(EffectDescriptor effect, CancellationTokenSource interrupter) {
            this.ActiveEffects.AddOrUpdate(
                effect, _ => new List<CancellationTokenSource> { interrupter }, 
                (_, interrupters) => {
                    interrupters.Add(interrupter);
                    return interrupters;
                }
            );
        }

        internal CancellationTokenSource Register(EffectDescriptor effect, CancellationToken interrupt) {
            CancellationTokenSource @internal = new CancellationTokenSource();
            this.Add(effect, @internal);
            return CancellationTokenSource.CreateLinkedTokenSource(interrupt, @internal.Token);
        }
        
        internal CancellationTokenSource Register(ISet<EffectDescriptor> effects, CancellationToken interrupt) {
            CancellationTokenSource @internal = new CancellationTokenSource();
            foreach (EffectDescriptor effect in effects) {
                this.Add(effect, @internal);
            }
            
            return CancellationTokenSource.CreateLinkedTokenSource(interrupt, @internal.Token);
        }

        internal void Stop(Ability? ability = null, Effect? type = null, Keyword keyword = default) {
            this.mutex.EnterWriteLock();
            try {
                IEnumerable<EffectDescriptor> matches =
                        this.ActiveEffects.Keys.Where(effect => effect.IsOnePossibleCaseOf(ability, type, keyword));
                foreach (EffectDescriptor match in matches) {
                    if (!this.ActiveEffects.TryRemove(match, out List<CancellationTokenSource> interrupters)) {
                        continue;
                    }

                    foreach (CancellationTokenSource interrupter in interrupters) {
                        interrupter.Cancel();
                        interrupter.Dispose();
                    }
                }
            } finally {
                this.mutex.ExitWriteLock();
            }
        }
    }
}
