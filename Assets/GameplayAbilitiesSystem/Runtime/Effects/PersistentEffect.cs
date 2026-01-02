using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal sealed class PersistentEffect : ContinuousEffect {
        private ICollection<Modifier> AppliedModifiers { get; } = new List<Modifier>();

        internal override void Reset() {
            base.Reset();
            this.AppliedModifiers.Clear();
        }

        internal async void Apply(Arguments args) {
            try {
                this.Apply(args.Target, args.RemovedKeywords, args.NewKeywords);
                foreach (Modifier modifier in args.Modifiers) {
                    this.AppliedModifiers.Add(modifier);
                    args.Target.ModifierConsumer.AddModifier(modifier);
                }
                
                await Awaitable.WaitForSecondsAsync(args.Duration, this.CancellationToken);
                this.Stop();
            } catch (OperationCanceledException) when (this.IsAlive) { } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        internal override void Stop() {
            foreach (Modifier modifier in this.AppliedModifiers) {
                this.Target.ModifierConsumer.AddModifier(-modifier);
            }
            
            base.Stop();
        }
        
        internal readonly record struct Arguments(
            IEffectReceiverFacade Target,
            float Duration,
            IEnumerable<Modifier> Modifiers,
            IEnumerable<string> RemovedKeywords,
            IEnumerable<string> NewKeywords
        );
    }
}