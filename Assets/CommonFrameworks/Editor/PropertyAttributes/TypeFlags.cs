using System;

namespace CommonFrameworks.Editor.PropertyAttributes {
    [Flags]
    public enum TypeFlags {
        None = 0,
        ConcreteClass = 1,
        AbstractClass = 1 << 1,
        Struct = 1 << 2,
        Enum = 1 << 3,
        Interface = 1 << 4
    }
}
