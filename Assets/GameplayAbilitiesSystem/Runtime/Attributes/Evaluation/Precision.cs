using SaintsField;

namespace GameplayAbilitiesSystem.Runtime.Attributes.Evaluation {
    public enum Precision {
        Integer = 0,
        
        [LabelText("To 1 decimal place")]
        OneDecimal = 1,
        
        [LabelText("To 2 decimal places")]
        TwoDecimals = 2,
        
        [LabelText("To 3 decimal places")]
        ThreeDecimals = 3,
    }
}