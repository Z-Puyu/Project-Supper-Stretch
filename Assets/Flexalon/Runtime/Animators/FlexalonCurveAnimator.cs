using Flexalon.Runtime.Core;
using UnityEngine;

namespace Flexalon.Runtime.Animators
{
    /// <summary>
    /// The curve animator applies a curve the the position, rotation, and scale
    /// of the object. The curve is restarted each time the layout position changes.
    /// This is ideal for scenarios in which the layout position does not change often.
    /// </summary>
    [AddComponentMenu("Flexalon/Flexalon Curve Animator"), HelpURL("https://www.flexalon.com/docs/animators")]
    public class FlexalonCurveAnimator : MonoBehaviour, TransformUpdater
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
        private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        /// <summary> The curve to apply. Should begin at 0 and end at 1. </summary>
        public AnimationCurve Curve
        {
            get => this._curve;
            set { this._curve = value; }
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
        /// <summary> Determines if the scale should be animated. </summary>
        public bool AnimateScale
        {
            get => this._animateScale;
            set { this._animateScale = value; }
        }

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _startScale;
        private Vector2 _startRectSize;

        private Vector3 _endPosition;
        private Quaternion _endRotation;
        private Vector3 _endScale;
        private Vector2 _endRectSize;

        private float _positionTime;
        private float _rotationTime;
        private float _scaleTime;
        private float _rectSizeTime;

        private Vector3 _fromPosition;
        private Quaternion _fromRotation;
        private Vector3 _fromScale;
        private Vector2 _fromRectSize;

        void OnEnable()
        {
            this._startPosition = this._endPosition = new Vector3(float.NaN, float.NaN, float.NaN);
            this._startRotation = this._endRotation = new Quaternion(float.NaN, float.NaN, float.NaN, float.NaN);
            this._startScale = this._endScale = new Vector3(float.NaN, float.NaN, float.NaN);
            this._positionTime = this._rotationTime = this._scaleTime = 0;
            this._rectTransform = (this.transform is RectTransform) ? (RectTransform)this.transform : null;

            this._node = Core.Flexalon.GetOrCreateNode(this.gameObject);
            this._node.SetTransformUpdater(this);
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
            this._fromRectSize = this._rectTransform?.rect.size ?? Vector2.zero;
        }

        /// <inheritdoc />
        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            var newEndPosition = position;
            var newStartPosition = this.transform.localPosition;

            if (this._animateInWorldSpace)
            {
                newEndPosition = this.transform.parent ? this.transform.parent.localToWorldMatrix.MultiplyPoint(position) : position;;
                newStartPosition = this._fromPosition;
            }

            if (newEndPosition != this._endPosition)
            {
                this._startPosition = newStartPosition;
                this._endPosition = newEndPosition;
                this._positionTime = 0;
            }

            this._positionTime += Time.smoothDeltaTime;

            if (!this._animatePosition || this._positionTime > this._curve.keys[this._curve.keys.Length - 1].time)
            {
                this.transform.localPosition = position;
                this._endPosition = new Vector3(float.NaN, float.NaN, float.NaN);
                return true;
            }
            else
            {
                var newPosition = Vector3.Lerp(this._startPosition, this._endPosition, this._curve.Evaluate(this._positionTime));
                if (this._animateInWorldSpace)
                {
                    this.transform.position = newPosition;
                }
                else
                {
                    this.transform.localPosition = newPosition;
                }

                return false;
            }
        }

        /// <inheritdoc />
        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {
            var newEndRotation = rotation;
            var newStartRotation = this.transform.localRotation;

            if (this._animateInWorldSpace)
            {
                newEndRotation = this.transform.parent ? this.transform.parent.rotation * rotation : rotation;;
                newStartRotation = this._fromRotation;
            }

            if (newEndRotation != this._endRotation)
            {
                this._startRotation = newStartRotation;
                this._endRotation = newEndRotation;
                this._rotationTime = 0;
            }

            this._rotationTime += Time.smoothDeltaTime;

            if (!this._animateRotation || this._rotationTime > this._curve.keys[this._curve.keys.Length - 1].time)
            {
                this.transform.localRotation = rotation;
                this._endRotation = new Quaternion(float.NaN, float.NaN, float.NaN, float.NaN);
                return true;
            }
            else
            {
                var newRotation = Quaternion.Slerp(this._startRotation, this._endRotation, this._curve.Evaluate(this._rotationTime));
                if (this._animateInWorldSpace)
                {
                    this.transform.rotation = newRotation;
                }
                else
                {
                    this.transform.localRotation = newRotation;
                }

                return false;
            }
        }

        /// <inheritdoc />
        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            var newEndScale = scale;
            var newStartScale = this.transform.localScale;

            if (this._animateInWorldSpace)
            {
                newEndScale = this.transform.parent ? Math.Mul(scale, this.transform.parent.lossyScale) : scale;
                newStartScale = this._fromScale;
            }

            if (newEndScale != this._endScale)
            {
                this._startScale = newStartScale;
                this._endScale = newEndScale;
                this._scaleTime = 0;
            }

            this._scaleTime += Time.smoothDeltaTime;

            if (!this._animateScale || this._scaleTime > this._curve.keys[this._curve.keys.Length - 1].time)
            {
                this.transform.localScale = scale;
                this._endScale = new Vector3(float.NaN, float.NaN, float.NaN);
                return true;
            }
            else
            {
                var newScale = Vector3.Lerp(this._startScale, this._endScale, this._curve.Evaluate(this._scaleTime));

                if (this._animateInWorldSpace)
                {
                    this.transform.localScale = this.transform.parent ? Math.Div(newScale, this.transform.parent.lossyScale) : newScale;
                }
                else
                {
                    this.transform.localScale = newScale;
                }

                return false;
            }
        }

        /// <inheritdoc />
        public bool UpdateRectSize(FlexalonNode node, Vector2 size)
        {
            if (size != this._endRectSize)
            {
                this._startRectSize = this._fromRectSize;
                this._endRectSize = size;
                this._rectSizeTime = 0;
            }

            this._rectSizeTime += Time.smoothDeltaTime;
            bool done = !this._animateScale || this._rectSizeTime > this._curve.keys[this._curve.keys.Length - 1].time;
            var newSize = done ? size : Vector2.Lerp(this._startRectSize, this._endRectSize, this._curve.Evaluate(this._rectSizeTime));
            this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
            this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);

            if (done)
            {
                this._endRectSize = new Vector2(float.NaN, float.NaN);
            }

            return done;
        }
    }
}