using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    public enum ApproximationPolicy {
        Truncate,
        
        [InspectorName("Round to nearest")]
        RoundToNearest,
        
        [InspectorName("Round up")]
        RoundUp,
        
        [InspectorName("Round down")]
        RoundDown,
    }
}