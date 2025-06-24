using UnityEngine;
using UnityEngine.Rendering;

namespace Flexalon.Samples.Runtime
{
    [ExecuteAlways]
    public class SampleMaterialPicker : MonoBehaviour
    {
        public Material Standard;
        public Material URP;
        public Material HDRP;

        void OnEnable()
        {
            var renderer = this.GetComponent<MeshRenderer>();
            if (renderer)
            {
                if (renderer.sharedMaterial != null && renderer.sharedMaterial != this.Standard && renderer.sharedMaterial != this.URP && renderer.sharedMaterial != this.HDRP)
                {
                    return;
                }

#if UNITY_6000_0_OR_NEWER
                var renderPipeline = GraphicsSettings.defaultRenderPipeline;
#else
                var renderPipeline = GraphicsSettings.renderPipelineAsset;
#endif
                if (renderPipeline?.GetType().Name.Contains("HDRenderPipelineAsset") ?? false)
                {
                    renderer.sharedMaterial = this.HDRP;
                }
                else if (renderPipeline?.GetType().Name.Contains("UniversalRenderPipelineAsset") ?? false)
                {
                    renderer.sharedMaterial = this.URP;
                }
                else
                {
                    renderer.sharedMaterial = this.Standard;
                }
            }
        }
    }
}