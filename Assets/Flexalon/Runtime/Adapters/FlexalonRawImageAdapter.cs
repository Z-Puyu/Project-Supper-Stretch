#if UNITY_UI

using Flexalon.Runtime.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Flexalon.Runtime.Adapters
{
    // An adapter which maintains the aspect ratio of a Texture on a RawImage
    // Note: by default Unity resizes textures to be power of 2, so either
    // change the texture import settings, or use a FlexalonAspectRatioAdapter.
    [ExecuteAlways, RequireComponent(typeof(RawImage))]
    public class FlexalonRawImageAdapter : FlexalonComponent, Adapter
    {
        private RawImage _rawImage;
        private Texture _texture;

        private void Awake()
        {
            this._rawImage = this.GetComponent<RawImage>();
        }

        // Returns a size which will maintain the aspect ratio of whichever
        // axis is set to SizeType.Component.
        public Bounds Measure(FlexalonNode node, Vector3 size, Vector3 min, Vector3 max)
        {
            if (this._texture)
            {
                var textureSize = new Vector2(this._texture.width, this._texture.height);
                return Math.MeasureComponentBounds2D(new Bounds(Vector3.zero, textureSize), node, size, min, max);
            }

            return new Bounds(Vector3.zero, size);
        }

        public bool TryGetScale(FlexalonNode node, out Vector3 scale)
        {
            scale = Vector3.one;
            return true;
        }

        public bool TryGetRectSize(FlexalonNode node, out Vector2 rectSize)
        {
            rectSize = node.Result.AdapterBounds.size;
            return true;
        }

        protected override void UpdateProperties()
        {
            this._node.SetAdapter(this);
        }

        protected override void ResetProperties()
        {
            this._node.SetAdapter(null);
        }

        public override void DoUpdate()
        {
            if (this._rawImage.texture != this._texture)
            {
                this._texture = this._rawImage.texture;
                this.MarkDirty();
            }
        }
    }
}

#endif