using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.GameManagement;
using Project.Scripts.Util.Singleton;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class WorldSpaceWidget : MonoBehaviour {
    [NotNull]
    [field: SerializeField]
    private Canvas? Widget { get; set; }
    
    [NotNull]
    [field: SerializeField, Required] 
    private Transform? Anchor { get; set; }
    
    [field: SerializeField] private float Height { get; set; } = 1;
    [field: SerializeField] private bool IsBillboard { get; set; }

    protected void Awake() {
        if (!this.Widget) {
            this.Widget = this.GetComponentInChildren<Canvas>();
        }
        
        if (!this.Anchor) {
            this.Anchor = this.transform.parent;
        }
    }

    private void OnEnable() {
        this.transform.position = this.Anchor.position + Vector3.up * this.Height;
    }

    protected void Start() {
        if (!this.Widget.worldCamera) {
            this.Widget.worldCamera = Singleton<GameInstance>.Instance.Eyes.GetComponent<Camera>();
        }
    }

    private void LateUpdate() {
        this.transform.position = this.Anchor.position + Vector3.up * this.Height;
        if (!this.IsBillboard || !this.Widget.gameObject.activeInHierarchy) {
            return;
        }
        
        this.transform.LookAt(Singleton<GameInstance>.Instance.Eyes);
        this.transform.Rotate(0, 180, 0);
    }
}
