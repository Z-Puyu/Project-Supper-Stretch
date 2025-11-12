using UnityEngine;

namespace UnityTechnologies.ParticlePack.Shared.Ramps.Runtime {
    [CreateAssetMenu]
    public class RampAsset : ScriptableObject
    {
        public Gradient gradient = new Gradient();
        public int size = 16;
        public bool up = false;
        public bool overwriteExisting = true;
    }
}
