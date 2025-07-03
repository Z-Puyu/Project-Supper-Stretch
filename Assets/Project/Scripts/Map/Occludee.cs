using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Util.Linq;
using UnityEngine;

namespace Project.Scripts.Map;

[DisallowMultipleComponent, RequireComponent(typeof(MeshRenderer))]
public class Occludee : MonoBehaviour {
    [NotNull] private MeshRenderer? Renderer { get; set; }
    public Vector3 Centre { get; private set; }
    private Vector3 Size { get; set; }
    private Face[] Faces { get; init; } = new Face[6];

    public bool IsOccluded {
        get => !this.Renderer.enabled;
        set => this.Renderer.enabled = !value;
    }


    private void Awake() {
        this.Renderer = this.GetComponent<MeshRenderer>();
    }

    private void Start() {
        this.ReadGeometry(this.Renderer.bounds);
    }

    private void OnDestroy() {
        if (Camera.main && Camera.main.TryGetComponent(out OcclusionCulling culling)) {
            culling.Unregister(this);
        }
    }

    private void ReadGeometry(Bounds bounds) {
        this.Centre = bounds.center;
        this.Size = bounds.size;
        Vector3[] vertices = [
            this.Centre + new Vector3(-this.Size.x, -this.Size.y, -this.Size.z),
            this.Centre + new Vector3(+this.Size.x, -this.Size.y, -this.Size.z),
            this.Centre + new Vector3(-this.Size.x, +this.Size.y, -this.Size.z),
            this.Centre + new Vector3(+this.Size.x, +this.Size.y, -this.Size.z),
            this.Centre + new Vector3(-this.Size.x, -this.Size.y, +this.Size.z),
            this.Centre + new Vector3(+this.Size.x, -this.Size.y, +this.Size.z),
            this.Centre + new Vector3(-this.Size.x, +this.Size.y, +this.Size.z),
            this.Centre + new Vector3(+this.Size.x, +this.Size.y, +this.Size.z)
        ];

        this.Faces[0] = new Face(vertices[4], vertices[6], vertices[7], vertices[5]);
        this.Faces[1] = new Face(vertices[1], vertices[3], vertices[2], vertices[0]);
        this.Faces[2] = new Face(vertices[0], vertices[2], vertices[6], vertices[4]);
        this.Faces[3] = new Face(vertices[5], vertices[7], vertices[3], vertices[1]);
        this.Faces[4] = new Face(vertices[2], vertices[3], vertices[7], vertices[6]);
        this.Faces[5] = new Face(vertices[0], vertices[4], vertices[5], vertices[1]);
    }

    public IEnumerable<Vector3> SamplePoints(Transform eye) {
        Bounds bounds = this.Renderer.bounds;
        if (bounds.center != this.Centre || bounds.size != this.Size) {
            this.ReadGeometry(bounds);
        }
        
        HashSet<Vector3> points = [this.Centre];
        foreach (Face face in this.Faces) {
            if (!face.IsFacing(eye)) {
                continue;
            }

            points.Add(face.Centre);
            face.Vertices.ForEach(v => points.Add(v));
        }

        return points;
    }

    public bool IsInCameraFrustum(Camera cam) {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), this.Renderer.bounds);
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = this.IsOccluded ? Color.red : Color.green;
        Bounds bounds = this.Renderer.bounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    private void OnValidate() {
        if (!this.Renderer) {
            this.Renderer = this.GetComponent<MeshRenderer>();
        }
    }
}
