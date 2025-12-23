using CommonFrameworks.Extensions;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Actions;

[DisallowMultipleComponent]
public sealed class ActionPerformer : MonoBehaviour {
    private ActionFlags Flags { get; set; } = ActionFlags.None;

    public void AddFlag(ActionFlags flag) {
        this.Flags |= flag;
    }
    
    public bool HasFlag(ActionFlags flag) {
        return this.Flags.Contains(flag);
    }
    
    public void ClearFlags() {
        this.Flags = ActionFlags.None;
    }
    
    public void RemoveFlag(ActionFlags flag) {
        this.Flags &= ~flag;
    }
}
