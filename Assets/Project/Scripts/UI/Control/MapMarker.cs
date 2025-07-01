using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.GameManagement;
using Project.Scripts.UI.Control.Minimap;
using Project.Scripts.Util.Linq;
using Project.Scripts.Util.Singleton;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class MapMarker : MonoBehaviour {
    [NotNull] protected static Transform? CameraTransform { get; set; }

    protected float VerticalDistanceToPlayer { get; private set; }

    [NotNull]
    [field: SerializeField, Required]
    protected Transform? Anchor { get; set; }

    [field: SerializeField] public string Name { get; private set; } = string.Empty;

    [field: SerializeField, MinValue(0.02f)]
    protected float DistanceBelowCamera { get; private set; } = 0.02f;
    
    [NotNull]
    [field: SerializeField, Required] 
    private WorldSpaceWidget? MarkerWidget { get; set; }
    
    [NotNull]
    [field: SerializeField, Required] 
    protected RectTransform? IconTransform { get; private set; }
    
    protected Vector2 CanvasScale { get; private set; }

    private void Awake() {
        MapMarker.CameraTransform = Singleton<MinimapCamera>.Instance.transform;
        this.CanvasScale = this.IconTransform.GetComponentInParent<Canvas>().transform.localScale;
    }

    private void OnEnable() {
        this.VerticalDistanceToPlayer =
                Mathf.Abs(this.Anchor.position.y - Singleton<GameInstance>.Instance.PlayerTransform.position.y);
        this.UpdatePosition();
    }

    private void OnDisable() {
        this.transform.position = this.Anchor.position;
    }

    protected virtual void UpdatePosition() {
        if (this.VerticalDistanceToPlayer >= Singleton<MinimapCamera>.Instance.HeightDivision) {
            this.transform.position = this.Anchor.position;
            this.MarkerWidget.gameObject.SetActive(false);
        } else {
            this.MarkerWidget.gameObject.SetActive(true);
            this.transform.position = this.Anchor.position with {
                y = MapMarker.CameraTransform.position.y - this.DistanceBelowCamera
            };
        }
    }

    protected void Update() {
        this.VerticalDistanceToPlayer =
                Mathf.Abs(this.Anchor.position.y - Singleton<GameInstance>.Instance.PlayerTransform.position.y);
        this.UpdatePosition();
        this.transform.forward = Singleton<MinimapCamera>.Instance.transform.forward;
    }
}
