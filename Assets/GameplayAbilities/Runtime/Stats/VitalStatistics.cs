using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects;
using UnityEngine;

namespace GameplayAbilities.Stats {
    internal sealed class VitalStatistics : MonoBehaviour {
        [NotNull] [field: SerializeField] private AttributeSet? AttributeSet { get; set; }
        [NotNull] [field: SerializeField] private EffectReceiver? EffectReceiver { get; set; }
        [NotNull] [field: SerializeField] private IAttributeReader? RegenSource { get; set; }
        [field: SerializeField] private List<VitalStat> Stats { get; set; } = new List<VitalStat>();

        private void Start() {
            foreach (VitalStat stat in this.Stats) {
                stat.Watch(this.AttributeSet, this.RegenSource, this.EffectReceiver);
            }
        }
    }
}
