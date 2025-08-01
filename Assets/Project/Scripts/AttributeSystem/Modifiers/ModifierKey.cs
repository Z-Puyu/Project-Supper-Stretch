namespace Project.Scripts.AttributeSystem.Modifiers;

public readonly record struct ModifierKey(string Target, ModifierType Type, int Duration) {
    public override string ToString() {
        return $"{this.Target} {this.Type}";
    }
}
