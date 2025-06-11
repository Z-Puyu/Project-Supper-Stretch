using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.UI.Control;

public class WorldSpaceWidget : MonoBehaviour {
    [NotNull]
    private Transform? CameraTransform { get; set; }
    
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
            this.Widget.worldCamera = Camera.main;
        }
        
        this.CameraTransform = this.Widget.worldCamera!.transform;
    }

    private void Update() {
        if (!this.IsBillboard || !this.Widget.gameObject.activeInHierarchy) {
            return;
        }
        
        this.transform.LookAt(this.CameraTransform);
        this.transform.Rotate(0, 180, 0);
    }
}
