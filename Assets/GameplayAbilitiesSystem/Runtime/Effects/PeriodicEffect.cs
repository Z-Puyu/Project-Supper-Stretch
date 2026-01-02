using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal sealed class PeriodicEffect : ContinuousEffect {
        private Awaitable? OngoingInterval { get; set; }

        internal override void Reset() {
            this.OngoingInterval = null;
            base.Reset();
        }
        
        internal async void Apply(Arguments args) {
            try {
                this.Apply(args.Target, args.RemovedKeywords, args.NewKeywords);
                if (args.ShouldImmediatelyTickOnApply) {
                    foreach (Modifier modifier in args.Modifiers) {
                        args.Target.ModifierConsumer.AddModifier(modifier);
                    }
                }

                int remainingTicks = args.TickCount;
                while (remainingTicks > 0) {
                    await Awaitable.WaitForSecondsAsync(args.Interval, this.CancellationToken);
                    foreach (Modifier modifier in args.Modifiers) {
                        args.Target.ModifierConsumer.AddModifier(modifier);
                    }

                    remainingTicks -= 1;
                }

                this.Stop();
            } catch (OperationCanceledException) when (this.IsAlive) { } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        internal readonly record struct Arguments(
            IEffectReceiverFacade Target,
            float Interval,
            int TickCount,
            bool ShouldImmediatelyTickOnApply,
            IEnumerable<Modifier> Modifiers,
            IEnumerable<string> RemovedKeywords,
            IEnumerable<string> NewKeywords
        );
    }
}