namespace Project.Scripts.AttributeSystem.Modifiers;

public readonly record struct ModifierKey(string Target, ModifierType Type) {
    public override string ToString() {
        return $"{this.Target} {this.Type}";
    }
}
