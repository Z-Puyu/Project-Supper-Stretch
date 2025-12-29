using System;

namespace GameCharacterBehaviours.Runtime.Actions {
    [Flags]
    public enum ActionFlags {
        None = 0,
        ActionOngoing = 1,
        RotationBlocked = 2,
        MovementBlocked = 4
    }
}
