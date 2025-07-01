using UnityEngine;

namespace Project.Scripts.Map;

public readonly record struct Face(Vector3[] Vertices, Vector3 Centre, Vector3 OutwardNormal) {
    public Face(Vector3 a, Vector3 b, Vector3 c, Vector3 d) : this([a, b, c, d], (a + b + c + d) / 4,
        Vector3.Cross(b - a, c - a).normalized) { }
    
    public bool IsFacing(Transform transform) {
        return Vector3.Dot(this.OutwardNormal, transform.forward) < 0;
    }

    public bool IsFacing(Vector3 direction) {
        return Vector3.Dot(this.OutwardNormal, direction) > 0;
    }
}
