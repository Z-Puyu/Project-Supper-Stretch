using System;
using Project.Scripts.GameManagement;
using Project.Scripts.UI.Control.Game.Minimap;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent, RequireComponent(typeof(WorldSpaceWidget))]
public class MapMarker : MonoBehaviour {
    [field: SerializeField] public string Name { get; private set; } = string.Empty;
    [field: SerializeField] public bool IsStatic { get; private set; } = true;
    private Vector3 InitialForwardDirection { get; set; }

    private void Start() {
        this.InitialForwardDirection = this.transform.forward;
    }

    private void Update() {
        if (this.IsStatic) {
            Vector3 forward = Singleton<MinimapCamera>.Instance.transform.up;
            this.transform.LookAt(forward with { x = -forward.x });
        } else {
            this.transform.forward = Singleton<MinimapCamera>.Instance.transform.forward;
        }
    }
}
