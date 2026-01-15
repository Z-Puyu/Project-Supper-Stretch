namespace CommonFrameworks.Blackboard {
    public readonly record struct BlackboardKey(string Name) {
        public static implicit operator BlackboardKey(string key) => new BlackboardKey(key);
        public static bool operator ==(BlackboardKey left, string right) => left.Name == right;
        public static bool operator !=(BlackboardKey left, string right) => left.Name != right;
    }
}
