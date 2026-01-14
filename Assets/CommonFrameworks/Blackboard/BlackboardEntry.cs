using System;

namespace CommonFrameworks.Blackboard {
    public readonly record struct BlackboardEntry<T>(T Value, Type DataType) {
        public BlackboardEntry(T value) : this(value, typeof(T)) { }
        
        public static implicit operator T(BlackboardEntry<T> entry) => entry.Value;
    }
}
