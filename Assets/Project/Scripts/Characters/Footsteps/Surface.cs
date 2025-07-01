using UnityEngine;

namespace Project.Scripts.Characters.Footsteps;

[DisallowMultipleComponent]
public class Surface : MonoBehaviour {
    [field: SerializeField] public SurfaceData? SurfaceData { get; private set; }
}
