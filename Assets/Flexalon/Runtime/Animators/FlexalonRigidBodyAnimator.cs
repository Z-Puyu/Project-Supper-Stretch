#if UNITY_PHYSICS

using Flexalon.Runtime.Core;
using UnityEngine;

namespace Flexalon.Runtime.Animators
{
    /// <summary>
    /// If you add a Rigid Body or Rigid Body 2D component a gameObject which is managed by Flexalon, then
    /// the physics system will fight with Flexalon over the object's position and rotation.
    /// Adding a Rigid Body animator will resolve this by applying forces to the the rigid body component
    /// instead of changing the transform directly.
    /// </summary>
    [AddComponentMenu("Flexalon/Flexalon Rigid Body Animator"), HelpURL("https://www.flexalon.com/docs/animators")]
    public class FlexalonRigidBodyAnimator : MonoBehaviour, TransformUpdater
    {
        private FlexalonNode _node;
        private Rigidbody _rigidBody;
        private Rigidbody2D _rigidBody2D;

        [SerializeField]
        private float _positionForce = 5.0f;
        /// <summary> How much force should be applied each frame to move the object to the layout position. </summary>
        public float PositionForce
        {
            get => this._positionForce;
            set { this._positionForce = value; }
        }

        [SerializeField]
        private float _rotationForce = 5.0f;
        /// <summary> How much force should be applied each frame to rotation the object to the layout rotation. </summary>
        public float RotationForce
        {
            get => this._rotationForce;
            set { this._rotationForce = value; }
        }

        [SerializeField]
        private float _scaleInterpolationSpeed = 5.0f;
        /// <summary> Amount the object's scale should be interpolated towards the layout size at each frame.
        /// This value is multiplied by Time.deltaTime. </summary>
        public float ScaleInterpolationSpeed
        {
            get => this._scaleInterpolationSpeed;
            set { this._scaleInterpolationSpeed = value; }
        }

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _fromScale;
        private RectTransform _rectTransform;

        void OnEnable()
        {
            this._node = Core.Flexalon.GetOrCreateNode(this.gameObject);
            this._node.SetTransformUpdater(this);
            this._rigidBody = this.GetComponent<Rigidbody>();
            this._rigidBody2D = this.GetComponent<Rigidbody2D>();
            this._targetPosition = this.transform.localPosition;
            this._targetRotation = this.transform.localRotation;
            this._rectTransform = (this.transform is RectTransform) ? (RectTransform)this.transform : null;
        }

        void OnDisable()
        {
            this._node.SetTransformUpdater(null);
            this._node = null;
        }

        /// <inheritdoc />
        public void PreUpdate(FlexalonNode node)
        {
            this._fromScale = this.transform.lossyScale;
        }

        /// <inheritdoc />
        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            if (this._rigidBody || this._rigidBody2D)
            {
                this._targetPosition = position;
                return false;
            }
            else
            {
                this.transform.localPosition = position;
                return true;
            }
        }

        /// <inheritdoc />
        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {
            if (this._rigidBody || this._rigidBody2D)
            {
                this._targetRotation = rotation;
                return false;
            }
            else
            {
                this.transform.localRotation = rotation;
                return true;
            }
        }

        /// <inheritdoc />
        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            var worldScale = this.transform.parent == null ? scale : Math.Mul(scale, this.transform.parent.lossyScale);
            if (Vector3.Distance(this._fromScale, worldScale) < 0.001f)
            {
                this.transform.localScale = scale;
                return true;
            }
            else
            {
                var newWorldScale = Vector3.Lerp(this._fromScale, worldScale, this._scaleInterpolationSpeed * Time.smoothDeltaTime);
                this.transform.localScale = this.transform.parent == null ? newWorldScale : Math.Div(newWorldScale, this.transform.parent.lossyScale);
                return false;
            }
        }

        /// <inheritdoc />
        public bool UpdateRectSize(FlexalonNode node, Vector2 size)
        {
            bool done = Vector2.Distance(this._rectTransform.sizeDelta, size) < 0.001f;
            var newSize = done ? size : Vector2.Lerp(this._rectTransform.sizeDelta, size, this._scaleInterpolationSpeed * Time.smoothDeltaTime);
            this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
            this._rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
            return done;
        }

        void FixedUpdate()
        {
            if (!this._rigidBody && !this._rigidBody2D)
            {
                return;
            }

            bool hasLayout = this._node.Parent != null || (this._node.Constraint != null && this._node.Constraint.Target != null);
            if (!hasLayout)
            {
                return;
            }

            var worldPos = this.transform.parent ? this.transform.parent.localToWorldMatrix.MultiplyPoint(this._targetPosition) : this._targetPosition;
            var force = (worldPos - this.transform.position) * this._positionForce;
            var rot = Quaternion.Slerp(this.transform.localRotation, this._targetRotation, this._rotationForce * Time.deltaTime);
            var rotWorldSpace = (this.transform.parent?.rotation ?? Quaternion.identity) * rot;

            if (this._rigidBody)
            {
                this._rigidBody.AddForce(force, ForceMode.Force);
                this._rigidBody.MoveRotation(rotWorldSpace);
            }

            if (this._rigidBody2D)
            {
                this._rigidBody2D?.AddForce(force, ForceMode2D.Force);
                this._rigidBody2D.MoveRotation(rotWorldSpace);
            }
        }
    }
}

#endif