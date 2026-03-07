using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Pool;

namespace GameplayAbilities.Effects.Schedulers {
    [Serializable]
    internal sealed class PersistentExecution : IScheduler {
        private const float DurationPollIntervalSeconds = 0.1f;
        
        private static readonly ObjectPool<PersistentExecution> Pool = new ObjectPool<PersistentExecution>(
            () => new PersistentExecution(), defaultCapacity: 20, maxSize: 200
        );
        
        [field: SerializeField, Min(0)] 
        [field: Tooltip("Duration of the effect in seconds. Set to 0 for infinite duration.")]
        private float Duration { get; set; }
        
        private float ElapsedTime { get; set; }
        private float RemainingTime => this.Duration - this.ElapsedTime;

        private List<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers { get; } =
            new List<KeyValuePair<GameplayAttributeType, Modifier>>();

        public IScheduler Compose(EffectExecutionScheme effect, IScheduler execution) {
            throw new NotImplementedException();
        }

        EffectExecutionSchedule IScheduler.ExecutionSchedule => new EffectExecutionSchedule {
            NumberOfTicks = 0,
            TickInterval = 0,
            ShouldTickOnStart = true,
            PersistentDuration = this.Duration
        };

        EffectExecutionState IScheduler.CurrentState => new EffectExecutionState {
            RemainingTicks = 0,
            RemainingDuration = this.RemainingTime, 
            Modifiers = this.Modifiers
        };

        public IScheduler Schedule(EffectExecutionScheme scheme) {
            PersistentExecution execution = PersistentExecution.Pool.Get();
            execution.Modifiers.Clear();
            execution.Modifiers.AddRange(scheme.Modifiers);
            execution.Duration = scheme.ExecutionSchedule.PersistentDuration;
            execution.ElapsedTime = 0;
            return execution;
        }

        private PersistentExecution() { }

        async Awaitable IScheduler.Execute(ModifierEnvironment target, CancellationToken interrupt) {
            try {
                foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                    target.AddModifier(modifier.Key, modifier.Value);
                }

                if (this.Duration <= 0f) {
                    while (true) {
                        await Awaitable.WaitForSecondsAsync(PersistentExecution.DurationPollIntervalSeconds, interrupt);
                    }
                }
                
                while (this.ElapsedTime < this.Duration) {
                    float interval = Mathf.Min(PersistentExecution.DurationPollIntervalSeconds, this.RemainingTime);
                    await Awaitable.WaitForSecondsAsync(interval, interrupt);
                    this.ElapsedTime += interval;
                }
            } catch (OperationCanceledException) { } finally {
                foreach (KeyValuePair<GameplayAttributeType, Modifier> modifier in this.Modifiers) {
                    target.AddModifier(modifier.Key, -modifier.Value);
                }
                
                this.ReleaseToPool();
            }
        }

        private void ReleaseToPool() {
            this.Modifiers.Clear();
            this.Duration = 0;
            this.ElapsedTime = 0;
            PersistentExecution.Pool.Release(this);
        }
    }
}
