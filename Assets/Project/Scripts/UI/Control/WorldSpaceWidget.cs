using System.Diagnostics.CodeAnalysis;
using Project.Scripts.GameManagement;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class WorldSpaceWidget : MonoBehaviour {
    [NotNull]
    [field: SerializeField]
    private Canvas? Widget { get; set; }
    
    [field: SerializeField]
    private bool IsBillboard { get; set; }

    protected void Awake() {
        if (!this.Widget) {
            this.Widget = this.GetComponentInChildren<Canvas>();
        }
    }

    protected void Start() {
        if (!this.Widget.worldCamera) {
            this.Widget.worldCamera = Singleton<GameInstance>.Instance.Eyes.GetComponent<Camera>();
        }
    }

    private void LateUpdate() {
        if (!this.IsBillboard || !this.Widget.gameObject.activeInHierarchy) {
            return;
        }
        
        this.transform.LookAt(Singleton<GameInstance>.Instance.Eyes);
        this.transform.Rotate(0, 180, 0);
    }
}
