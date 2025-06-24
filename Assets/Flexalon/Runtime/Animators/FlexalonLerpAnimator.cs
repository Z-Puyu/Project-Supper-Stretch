using Flexalon.Runtime.Core;
using UnityEngine;

namespace Flexalon.Runtime.Animators
{
    /// <summary>
    /// The lerp animator constnatly performs a linear interpolation between
    /// the object's current position and its layout position. This is useful
    /// if the layout position is continuously changing.
    /// </summary>
    [AddComponentMenu("Flexalon/Flexalon Lerp Animator"), HelpURL("https://www.flexalon.com/docs/animators")]
    public class FlexalonLerpAnimator : MonoBehaviour, TransformUpdater
    {
        private FlexalonNode _node;
        private RectTransform _rectTransform;

        [SerializeField]
        private bool _animateInWorldSpace = true;
        /// <summary> Determines if the animation should be performed in world space. </summary>
        public bool AnimateInWorldSpace
        {
            get => this._animateInWorldSpace;
            set { this._animateInWorldSpace = value; }
        }

        [SerializeField]
        private float _interpolationSpeed = 5.0f;
        /// <summary> Amount the object should be interpolated towards the target at each frame.
        /// This value is multiplied by Time.deltaTime. </summary>
        public float InterpolationSpeed
        {
            get => this._interpolationSpeed;
            set { this._interpolationSpeed = value; }
        }

        [SerializeField]
        private bool _animatePosition = true;
        /// <summary> Determines if the position should be animated. </summary>
        public bool AnimatePosition
        {
            get => this._animatePosition;
            set { this._animatePosition = value; }
        }

        [SerializeField]
        private bool _animateRotation = true;
        /// <summary> Determines if the rotation should be animated. </summary>
        public bool AnimateRotation
        {
            get => this._animateRotation;
            set { this._animateRotation = value; }
        }

        [SerializeField]
        private bool _animateScale = true;
        /// <summary> Determines if the rotation should be animated. </summary>
        public bool AnimateScale
        {
            get => this._animateScale;
            set { this._animateScale = value; }
        }

        private Vector3 _fromPosition;
        private Quaternion _fromRotation;
        private Vector3 _fromScale;

        void OnEnable()
        {
            this._node = Core.Flexalon.GetOrCreateNode(this.gameObject);
            this._node.SetTransformUpdater(this);
            this._rectTransform = (this.transform is RectTransform) ? (RectTransform)this.transform : null;
        }

        void OnDisable()
        {
            this._node?.SetTransformUpdater(null);
            this._node = null;
        }

        /// <inheritdoc />
        public void PreUpdate(FlexalonNode node)
        {
            this._fromPosition = this.transform.position;
            this._fromRotation = this.transform.rotation;
            this._fromScale = this.transform.lossyScale;
        }

        /// <inheritdoc />
        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            if (this._animateInWorldSpace)
            {
                var worldPosition = this.transform.parent ? this.transform.parent.localToWorldMatrix.MultiplyPoint(position) : position;
                if (!this._animatePosition || Vector3.Distance(this._fromPosition, worldPosition) < 0.001f)
                {
                    this.transform.localPosition = position;
                    return true;
                }
                else
                {
                    this.transform.position = Vector3.Lerp(this._fromPosition, worldPosition, this._interpolationSpeed * Time.smoothDeltaTime);
                    return false;
                }
            }
            else
            {
                if (!this._animatePosition || Vector3.Distance(this.transform.localPosition, position) < 0.001f)
                {
                    this.transform.localPosition = position;
                    return true;
                }
                else
                {
                    this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, position, this._interpolationSpeed * Time.smoothDeltaTime);
                    return false;
                }
            }
        }

        /// <inheritdoc />
        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {
            if (this._animateInWorldSpace)
            {
                var worldRotation = this.transform.parent ? this.transform.parent.rotation * rotation : rotation;
                if (!this._animateRotation || Mathf.Abs(Quaternion.Angle(this._fromRotation, worldRotation)) < 0.001f)
                {
                    this.transform.localRotation = rotation;
                    return true;
                }
                else
                {
                    this.transform.rotation = Quaternion.Slerp(this._fromRotation, worldRotation, this._interpolationSpeed * Time.smoothDeltaTime);
                    return false;
                }
            }
            else
            {
                if (!this._animateRotation || Mathf.Abs(Quaternion.Angle(this.transform.localRotation, rotation)) < 0.001f)
                {
                    this.transform.localRotation = rotation;
                    return true;
                }
                else
                {
                    this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, rotation, this._interpolationSpeed * Time.smoothDeltaTime);
                    return false;
                }
            }
        }

        /// <inheritdoc />
        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            if (this._animateInWorldSpace)
            {
                var worldScale = this.transform.parent ? Math.Mul(scale, this.transform.parent.lossyScale) : scale;
                if (!this._animateScale || Vector3.Distance(this._fromScale, worldScale) < 0.001f)
                {
                    this.transform.localScale = scale;
                    return true;
                }
                else
                {
                    var newWorldScale = Vector3.Lerp(this._fromScale, worldScale, this._interpolationSpeed * Time.smoothDeltaTime);
                    this.transform.localScale = this.transform.parent ? Math.Div(newWorldScale, this.transform.parent.lossyScale) : newWorldScale;
                    return false;
                }
            }
            else
            {
                if (!this._animateScale || Vector3.Distance(this.transform.localScale, scale) < 0.001f)
                {
                    this.transform.localScale = scale;
                    return true;
                }
                else
                {
                    this.transform.localScale = Vector3.Lerp(this.transform.localScale, scale, this._interpolationSpeed * Time.smoothDeltaTime);
                    return false;
                }
            }
        }

        /// <inheritdoc />
        public bool UpdateRectSize(FlexalonNode node, Vector2 size)
        {
            bool done = !this._animateScale || Vector2.Distance(this._rectTransform.sizeDelta, size) < 0.001f;
            var newSize = done ? size : Vector2.Lerp(this._rectTransform.sizeDelta, size, this._interpolationSpeed * Time.smoothDeltaTime);
            this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
            this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
            return done;
        }
    }
}