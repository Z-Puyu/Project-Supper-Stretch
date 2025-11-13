using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public readonly struct RuntimeEffect : IEquatable<RuntimeEffect> {
        internal Effect SourceEffect { get; }
        private List<Modifier> Modifiers { get; }
        private List<Keyword> KeywordsToAdd { get; }
        private List<Keyword> KeywordsToRemove { get; }
        internal double Duration { get; }
        internal double Interval { get; }
        internal int TickCount { get; }
        private EffectSource Source { get; }
        private EffectTarget Target { get; }

        internal RuntimeEffect(
            Effect sourceEffect, IEnumerable<EffectModifier> modifiers, IEnumerable<Keyword> keywordsToAdd,
            IEnumerable<Keyword> keywordsToRemove, EffectSource source, EffectTarget target
        ) {
            this.SourceEffect = sourceEffect;
            this.Modifiers = new List<Modifier>();
            this.KeywordsToAdd = new List<Keyword>();
            this.KeywordsToRemove = new List<Keyword>();
            foreach (EffectModifier modifier in modifiers) {
                this.Modifiers.Add(modifier.CreateModifier(source, target));
            }

            this.KeywordsToAdd.AddRange(keywordsToAdd);
            this.KeywordsToRemove.AddRange(keywordsToRemove);
            this.Duration = 0;
            this.Interval = -1;
            this.TickCount = 0;
            this.Source = source;
            this.Target = target;
        }

        internal RuntimeEffect(
            Effect sourceEffect, IEnumerable<EffectModifier> modifiers, IEnumerable<Keyword> keywordsToAdd,
            IEnumerable<Keyword> keywordsToRemove, EffectSource source, EffectTarget target,
            double duration
        ) : this(sourceEffect, modifiers, keywordsToAdd, keywordsToRemove, source, target) {
            this.Duration = duration;
        }

        internal RuntimeEffect(
            Effect sourceEffect, IEnumerable<EffectModifier> modifiers, IEnumerable<Keyword> keywordsToAdd,
            IEnumerable<Keyword> keywordsToRemove, EffectSource source, EffectTarget target,
            double duration, double interval, int tickCount
        ) : this(sourceEffect, modifiers, keywordsToAdd, keywordsToRemove, source, target, duration) {
            this.Interval = interval;
            this.TickCount = tickCount;
        }

        public void Execute(KeywordContainer targetKeywordContainer, ModifierEnvironment targetModifierEnvironment) {
            foreach (Modifier modifier in this.Modifiers) {
                targetModifierEnvironment.AddModifier(modifier);
            }

            foreach (Keyword keyword in this.KeywordsToAdd) {
                targetKeywordContainer.Add(keyword);
            }

            foreach (Keyword keyword in this.KeywordsToRemove) {
                targetKeywordContainer.Remove(keyword);
            }
        }

        public bool Equals(RuntimeEffect other) {
            return Equals(this.SourceEffect, other.SourceEffect) && Equals(this.Modifiers, other.Modifiers) && Equals(this.KeywordsToAdd, other.KeywordsToAdd) && Equals(this.KeywordsToRemove, other.KeywordsToRemove) && this.Duration.Equals(other.Duration) && this.Interval.Equals(other.Interval) && this.TickCount == other.TickCount && this.Source.Equals(other.Source) && Equals(this.Target, other.Target);
        }

        public override bool Equals(object obj) {
            return obj is RuntimeEffect other && this.Equals(other);
        }

        public override int GetHashCode() {
            HashCode hashCode = new HashCode();
            hashCode.Add(this.SourceEffect);
            hashCode.Add(this.Modifiers);
            hashCode.Add(this.KeywordsToAdd);
            hashCode.Add(this.KeywordsToRemove);
            hashCode.Add(this.Duration);
            hashCode.Add(this.Interval);
            hashCode.Add(this.TickCount);
            hashCode.Add(this.Source);
            hashCode.Add(this.Target);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(RuntimeEffect left, RuntimeEffect right) {
            return left.Equals(right);
        }

        public static bool operator !=(RuntimeEffect left, RuntimeEffect right) {
            return !left.Equals(right);
        }
    }
}
