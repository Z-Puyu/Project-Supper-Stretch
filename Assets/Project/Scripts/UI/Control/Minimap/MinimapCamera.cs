using System.Diagnostics.CodeAnalysis;
using Project.Scripts.GameManagement;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control.Minimap;

public class MinimapCamera : Singleton<MinimapCamera> {
    private float CurrentHeight { get; set; }
    private float NextLevel { get; set; }
    private float PreviousLevel { get; set; }
    [field: SerializeField] private float HeightDivision { get; set; } = 4;
    [field: SerializeField] private MapMarker? PlayerMarker { get; set; }
    [NotNull] private Transform? PlayerTransform { get; set; }
    [NotNull] private Transform? CameraTransform { get; set; }

    private void Start() {
        this.CameraTransform = this.transform;
        this.PlayerTransform = this.CameraTransform.root;
        this.CurrentHeight = this.CameraTransform.position.y;
        this.NextLevel = this.PlayerTransform.position.y + this.HeightDivision;
        this.PreviousLevel = this.NextLevel - 2 * this.HeightDivision;
    }

    private void Update() {
        Vector3 forward = Singleton<GameInstance>.Instance.Eyes.TransformDirection(Vector3.forward) with { y = 0 };
        this.transform.LookAt(this.transform.position + forward);
    }
    
    private void LateUpdate() {
        Vector3 position = this.PlayerTransform.position;
        if (position.y >= this.NextLevel) {
            this.CurrentHeight += this.HeightDivision;
            this.NextLevel += this.HeightDivision;
            this.PreviousLevel += this.HeightDivision;       
        } else if (position.y <= this.PreviousLevel) {
            this.CurrentHeight -= this.HeightDivision;       
            this.PreviousLevel -= this.HeightDivision;       
            this.NextLevel -= this.HeightDivision;      
        }
        
        this.CameraTransform.position = this.CameraTransform.position with { y = this.CurrentHeight };
        
        if (this.PlayerMarker) {
            this.PlayerMarker.transform.position = this.CameraTransform.position - new Vector3(0, 0.02f, 0);
        }
    }
}
