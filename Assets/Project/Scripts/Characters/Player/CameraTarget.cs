using System;
using Project.Scripts.Common;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Player;

public class CameraTarget : MonoBehaviour {
    [field: SerializeField, MinValue(0)] private float AngularSpeed { get; set; } = 10;
    
    [field: SerializeField, MinMaxSlider(-90, 90)] 
    private Vector2 VerticalRange { get; set; } = new Vector2(-40, 70);
    
    private float Pitch { get; set; }
    private float Yaw { get; set; }
    private bool IsFrozen { get; set; }

    private void Start() {
        GameEvents.OnPause += this.Freeze;
        GameEvents.OnPlay += this.Unfreeze;
        Quaternion rotation = this.transform.rotation;
        this.Pitch = rotation.eulerAngles.x;
        this.Yaw = rotation.eulerAngles.y;   
    }

    private void OnDestroy() {
        GameEvents.OnPause -= this.Freeze;
        GameEvents.OnPlay -= this.Unfreeze;   
    }

    private void Freeze() {
        this.IsFrozen = true;
    }
    
    private void Unfreeze() {
        this.IsFrozen = false;   
    }

    private void LateUpdate() {
        if (this.IsFrozen) {
            return;
        }
        
        float x = Input.GetAxis("Mouse X") * this.AngularSpeed * Time.deltaTime;
        float y = -Input.GetAxis("Mouse Y") * this.AngularSpeed * Time.deltaTime;
        this.Pitch = Mathf.Clamp(this.Pitch + y, this.VerticalRange.x, this.VerticalRange.y);
        this.Yaw += x;
        this.transform.rotation = Quaternion.Euler(this.Pitch, this.Yaw, this.transform.rotation.eulerAngles.z);
    }
}
