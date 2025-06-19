using UnityEngine;

namespace Project.Scripts.UI.Components;

public class UIObject : MonoBehaviour {
    public virtual void Display(object? data) { }
    
    public virtual void Clear() { }
}
