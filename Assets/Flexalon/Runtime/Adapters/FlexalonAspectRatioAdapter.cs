using Flexalon.Runtime.Core;
using UnityEngine;

namespace Flexalon.Runtime.Adapters
{
    // A simple adapter which maintains a specified aspect ratio
    [ExecuteAlways]
    public class FlexalonAspectRatioAdapter : FlexalonComponent, Adapter
    {
        [SerializeField]
        private float _width;
        public float Width
        {
            get => this._width;
            set
            {
                this._width = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _height;
        public float Height
        {
            get => this._height;
            set
            {
                this._height = value;
                this.MarkDirty();
            }
        }

        public Bounds Measure(FlexalonNode node, Vector3 size, Vector3 min, Vector3 max)
        {
            return Math.MeasureComponentBounds2D(new Bounds(Vector3.zero, new Vector3(this._width, this._height, 1)), node, size, min, max);
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
    }
}