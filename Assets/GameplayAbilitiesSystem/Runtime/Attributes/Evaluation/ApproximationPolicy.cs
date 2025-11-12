using SaintsField;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    public enum ApproximationPolicy {
        Truncate,
        
        [LabelText("Round to nearest")]
        RoundToNearest,
        
        [LabelText("Round up")]
        RoundUp,
        
        [LabelText("Round down")]
        RoundDown,
    }
}
