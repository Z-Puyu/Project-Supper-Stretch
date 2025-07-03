using Project.Scripts.UI.Control.Minimap;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class MapGoal : MapMarker {
    protected override void UpdatePosition() {
        Vector3 cameraPosition = MapMarker.CameraTransform.position;
        Vector3 markerPosition = this.Anchor.position;
        Vector3 xzTranslation = MapMarker.CameraTransform
                                         .InverseTransformDirection((markerPosition - cameraPosition) with { y = 0 });
        Vector2 minimapHalfSize = Singleton<MinimapCamera>.Instance.HalfSize;
        Vector2 iconSize = this.IconTransform.rect.size * this.CanvasScale;
        float xOffset = Mathf.Clamp(xzTranslation.x, iconSize.x - minimapHalfSize.x, minimapHalfSize.x - iconSize.x);
        float zOffset = Mathf.Clamp(xzTranslation.z, iconSize.y - minimapHalfSize.y, minimapHalfSize.y - iconSize.y);
        Vector3 worldTranslation = MapMarker.CameraTransform.TransformDirection(new Vector3(xOffset, 0, zOffset));
        this.transform.position = (cameraPosition + worldTranslation) with {
            y = cameraPosition.y - this.DistanceBelowCamera
        };
    }
}
