using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Stats {
    internal sealed class VitalStatistics : MonoBehaviour {
        [NotNull] [field: SerializeField] private AttributeSet? AttributeSet { get; set; }
        [NotNull] [field: SerializeField] private EffectReceiver? EffectReceiver { get; set; }
        [field: SerializeField] private Ref<IAttributeReader> RegenSource { get; set; }
        [field: SerializeField] private List<VitalStat> Stats { get; set; } = new List<VitalStat>();

        private void Start() {
            IAttributeReader? source = this.RegenSource.Value;
            if (source == null) {
#if DEBUG
                Debug.LogWarning("Vital statistics regeneration source is not set", this);
#endif
                source = this.AttributeSet;
            }
            
            foreach (VitalStat stat in this.Stats) {
                stat.Watch(this.AttributeSet, source, this.EffectReceiver);
            }
        }
    }
}
