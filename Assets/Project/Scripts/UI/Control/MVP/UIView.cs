using UnityEngine;

namespace Project.Scripts.UI.Control.MVP;

[DisallowMultipleComponent]
public abstract class UIView : MonoBehaviour {
    public abstract void Refresh();

    public abstract void Clear();
}
