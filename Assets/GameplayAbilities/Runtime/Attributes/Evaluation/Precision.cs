using UnityEngine;

namespace GameplayAbilities.Attributes.Evaluation {
    public enum Precision {
        Integer = 0,
        [InspectorName("To 1 decimal place")] OneDecimal = 1,
        [InspectorName("To 2 decimal places")] TwoDecimals = 2,
        [InspectorName("To 3 decimal places")] ThreeDecimals = 3,
    }
}