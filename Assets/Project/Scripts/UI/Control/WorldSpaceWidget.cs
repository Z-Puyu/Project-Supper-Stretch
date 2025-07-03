using System.Diagnostics.CodeAnalysis;
using Project.Scripts.GameManagement;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class WorldSpaceWidget : MonoBehaviour {
    [NotNull]
    [field: SerializeField]
    private Canvas? Canvas { get; set; }
    
    [field: SerializeField] private bool IsBillboard { get; set; }

    protected void Awake() {
        if (!this.Canvas) {
            this.Canvas = this.GetComponentInChildren<Canvas>();
        }
    }

    protected void Start() {
        if (!this.Canvas.worldCamera) {
            this.Canvas.worldCamera = Singleton<GameInstance>.Instance.Eyes.GetComponent<Camera>();
        }
    }

    private void LateUpdate() {
        if (!this.IsBillboard || !this.Canvas.gameObject.activeInHierarchy) {
            return;
        }
        
        this.transform.LookAt(Singleton<GameInstance>.Instance.Eyes);
        this.transform.Rotate(0, 180, 0);
    }

    private void OnValidate() {
        if (!this.Canvas) {
            this.Canvas = this.GetComponentInChildren<Canvas>();
        }
    }
}
