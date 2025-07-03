using System.Diagnostics.CodeAnalysis;
using Project.Scripts.GameManagement;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control.Minimap;

public class MinimapCamera : Singleton<MinimapCamera> {
    private float CurrentHeight { get; set; }
    private float NextLevel { get; set; }
    private float PreviousLevel { get; set; }
    [field: SerializeField] public float HeightDivision { get; private set; } = 4;
    [NotNull] private Transform? PlayerTransform { get; set; }
    [NotNull] private Transform? CameraTransform { get; set; }
    public Vector2 HalfSize { get; private set; }

    private void Start() {
        this.CameraTransform = this.transform;
        this.PlayerTransform = this.CameraTransform.root;
        this.CurrentHeight = this.CameraTransform.position.y;
        this.NextLevel = this.PlayerTransform.position.y + this.HeightDivision;
        this.PreviousLevel = this.NextLevel - 2 * this.HeightDivision;
        Camera cam = this.GetComponentInChildren<Camera>(includeInactive: true);
        float halfHeight = cam.orthographicSize;
        this.HalfSize = new Vector2(halfHeight * cam.aspect, halfHeight);
    }

    private void Update() {
        Vector3 forward = Singleton<GameInstance>.Instance.Eyes.TransformDirection(Vector3.forward) with { y = 0 };
        this.transform.LookAt(this.transform.position + forward);
    }

    private void FollowPlayer() {
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
    }
    
    private void LateUpdate() {
        this.FollowPlayer();
    }
}
