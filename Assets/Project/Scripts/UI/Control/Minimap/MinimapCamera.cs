using Project.Scripts.GameManagement;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control.Minimap;

public class MinimapCamera : Singleton<MinimapCamera> {
    private void Update() {
        Vector3 forward = Singleton<GameInstance>.Instance.Eyes.TransformDirection(Vector3.forward) with { y = 0 };
        this.transform.LookAt(this.transform.position + forward);
    }
}
