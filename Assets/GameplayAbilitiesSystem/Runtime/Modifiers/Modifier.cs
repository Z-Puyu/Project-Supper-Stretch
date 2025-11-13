using GameplayAbilitiesSystem.Runtime.Attributes;
using Unity.VisualScripting;

namespace GameplayAbilitiesSystem.Runtime.Modifiers {
    public readonly struct Modifier {
        internal AttributeKey Target { get; }
        internal ModifierType Type { get; }
        internal ModifierValue Value { get; }
        
        internal Modifier(AttributeKey target, ModifierType type, ModifierValue value) {
            this.Target = target;
            this.Type = type;
            this.Value = value;
        }
        
        public static Modifier operator -(Modifier modifier) {
            return new Modifier(modifier.Target, modifier.Type, -modifier.Value);
        }
    }
}
