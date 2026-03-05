using GameplayAbilities.Editor.UI;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public sealed class ValidatePropertyPainter : PropertyPainter<ValidateAttribute> {
        private const string ValidatorElementName = "Validator";
        private const string ErrorMessage = "Invalid field value!";
        
        protected override void Paint(
            CustomisablePropertyField drawer, SerialisedData data, ValidateAttribute attribute
        ) {
            drawer.Bottom.Add(
                new HelpBox(ValidatePropertyPainter.ErrorMessage, HelpBoxMessageType.Error) {
                    name = ValidatePropertyPainter.ValidatorElementName, style = {
                        flexGrow = 1,
                        flexShrink = 0
                    }
                }
            );
            
            drawer.TrackPropertyValue(data.SerialisedProperty, validate);
            validate(data.SerialisedProperty);
            return;
            
            void validate(SerializedProperty property) {
                HelpBox? validator = drawer.Bottom.Q<HelpBox>(ValidatePropertyPainter.ValidatorElementName);
                if (validator is null) {
                    return;
                }

                bool pass = string.IsNullOrWhiteSpace(attribute.PredicateName) ||
                            data.ResolveCallback<bool>(attribute.PredicateName, data.Value);
                validator.style.display = pass ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }
    }
}
