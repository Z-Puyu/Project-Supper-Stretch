using UnityEngine;
using UnityEngine.Rendering;

namespace Flexalon.Samples.Runtime
{
    // Automatically selects the right material based on render pipeline and provides helpers for setting the color.
    [ExecuteAlways, AddComponentMenu("Flexalon Samples/Flexalon Dynamic Material")]
    public class FlexalonDynamicMaterial : MonoBehaviour
    {
        public Material Standard;
        public Material URP;
        public Material HDRP;

        [SerializeField]
        private Color _color = Color.white;
        public Color Color => this._color;

        private MeshRenderer _meshRenderer;

        void OnEnable()
        {
            this.UpdateMeshRenderer();
            if (this._meshRenderer)
            {
#if UNITY_6000_0_OR_NEWER
                var renderPipeline = GraphicsSettings.defaultRenderPipeline;
#else
                var renderPipeline = GraphicsSettings.renderPipelineAsset;
#endif
                if (renderPipeline?.GetType().Name.Contains("HDRenderPipelineAsset") ?? false)
                {
                    this._meshRenderer.sharedMaterial = this.HDRP;
                }
                else if (renderPipeline?.GetType().Name.Contains("UniversalRenderPipelineAsset") ?? false)
                {
                    this._meshRenderer.sharedMaterial = this.URP;
                }
                else
                {
                    this._meshRenderer.sharedMaterial = this.Standard;
                }

                this.SetColor(this._color);
            }
        }

        private string GetColorPropertyName()
        {
            if (this._meshRenderer.sharedMaterial.HasProperty("_BaseColor")) // HRDP.Lit / URP.Lit
            {
                return "_BaseColor";
            }
            else if (this._meshRenderer.sharedMaterial.HasProperty("_Color")) // Standard
            {
                return "_Color";
            }

            return null;
        }

        public void SetColor(Color color)
        {
            this._color = color;
            this.UpdateMeshRenderer();
            if (this._meshRenderer)
            {
                var propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetColor(this.GetColorPropertyName(), color);
                this._meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        private void UpdateMeshRenderer()
        {
            if (this._meshRenderer == null)
            {
                this._meshRenderer = this.GetComponent<MeshRenderer>();
            }
        }
    }
}