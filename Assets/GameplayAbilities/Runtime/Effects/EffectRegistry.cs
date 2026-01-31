using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace GameplayAbilities.Effects {
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

        internal CancellationToken Register(EffectDescriptor effect) {
            CancellationTokenSource @internal = new CancellationTokenSource();
            this.Add(effect, @internal);
            return @internal.Token;
        }

        internal void Stop(EffectDescriptor effect) {
            this.mutex.EnterWriteLock();
            try {
                List<EffectDescriptor> matches = new List<EffectDescriptor>();
                foreach (EffectDescriptor e in this.ActiveEffects.Keys) {
                    if (e.IsOnePossibleCaseOf(effect)) {
                        matches.Add(e);
                    }
                }

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
