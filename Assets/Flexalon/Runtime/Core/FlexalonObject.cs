using UnityEngine;

namespace Flexalon.Runtime.Core
{
    /// <summary> To control the size of an object, add a Flexalon Object
    /// component to it and edit the width, height, or depth properties. </summary>
    [DisallowMultipleComponent, AddComponentMenu("Flexalon/Flexalon Object"), HelpURL("https://www.flexalon.com/docs/flexalonObject")]
    public class FlexalonObject : FlexalonComponent
    {
        /// <summary> The fixed size of the object. </summary>
        public Vector3 Size
        {
            get => new Vector3(this._width, this._height, this._depth);
            set
            {
                this.Width = value.x;
                this.Height = value.y;
                this.Depth = value.z;
            }
        }

        /// <summary> The relative size of the object. </summary>
        public Vector3 SizeOfParent
        {
            get => new Vector3(this._widthOfParent, this._heightOfParent, this._depthOfParent);
            set
            {
                this.WidthOfParent = value.x;
                this.HeightOfParent = value.y;
                this.DepthOfParent = value.z;
            }
        }

        [SerializeField]
        private SizeType _widthType = SizeType.Component;
        /// <summary> The width type of the object. </summary>
        public SizeType WidthType
        {
            get { return this._widthType; }
            set {
                this._widthType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _width = 1;
        /// <summary> The fixed width of the object. </summary>
        public float Width
        {
            get { return this._width; }
            set {
                this._width = Mathf.Max(value, 0);
                this._widthType = SizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _widthOfParent = 1;
        /// <summary> The relative width of the object. </summary>
        public float WidthOfParent
        {
            get { return this._widthOfParent; }
            set {
                this._widthOfParent = Mathf.Max(value, 0);
                this._widthType = SizeType.Fill;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private SizeType _heightType = SizeType.Component;
        /// <summary> The height type of the object. </summary>
        public SizeType HeightType
        {
            get { return this._heightType; }
            set {
                this._heightType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _height = 1;
        /// <summary> The fixed height of the object. </summary>
        public float Height
        {
            get { return this._height; }
            set {
                this._height = Mathf.Max(value, 0);
                this._heightType = SizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _heightOfParent = 1;
        /// <summary> The relative height of the object. </summary>
        public float HeightOfParent
        {
            get { return this._heightOfParent; }
            set {
                this._heightOfParent = Mathf.Max(value, 0);
                this._heightType = SizeType.Fill;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private SizeType _depthType = SizeType.Component;
        /// <summary> The depth type of the object. </summary>
        public SizeType DepthType
        {
            get { return this._depthType; }
            set {
                this._depthType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _depth = 1;
        /// <summary> The fixed depth of the object. </summary>
        public float Depth
        {
            get { return this._depth; }
            set {
                this._depth = Mathf.Max(value, 0);
                this._depthType = SizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _depthOfParent = 1;
        /// <summary> The relative depth of the object. </summary>
        public float DepthOfParent
        {
            get { return this._depthOfParent; }
            set {
                this._depthOfParent = Mathf.Max(value, 0);
                this._depthType = SizeType.Fill;
                this.MarkDirty();
            }
        }

        /// <summary> The min fixed size of the object. </summary>
        public Vector3 MinSize
        {
            get => new Vector3(this._minWidth, this._minHeight, this._minDepth);
            set
            {
                this.MinWidth = value.x;
                this.MinHeight = value.y;
                this.MinDepth = value.z;
            }
        }

        /// <summary> The min relative size of the object. </summary>
        public Vector3 MinSizeOfParent
        {
            get => new Vector3(this._minWidthOfParent, this._minHeightOfParent, this._minDepthOfParent);
            set
            {
                this.MinWidthOfParent = value.x;
                this.MinHeightOfParent = value.y;
                this.MinDepthOfParent = value.z;
            }
        }

        [SerializeField]
        private MinMaxSizeType _minWidthType = MinMaxSizeType.None;
        /// <summary> The min width type of the object. </summary>
        public MinMaxSizeType MinWidthType
        {
            get { return this._minWidthType; }
            set {
                this._minWidthType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _minWidth = 0;
        /// <summary> The min fixed min width of the object. </summary>
        public float MinWidth
        {
            get { return this._minWidth; }
            set {
                this._minWidth = Mathf.Max(value, 0);
                this._minWidthType = MinMaxSizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _minWidthOfParent = 0;
        /// <summary> The min relative width of the object. </summary>
        public float MinWidthOfParent
        {
            get { return this._minWidthOfParent; }
            set {
                this._minWidthOfParent = Mathf.Max(value, 0);
                this._minWidthType = MinMaxSizeType.Fill;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private MinMaxSizeType _minHeightType = MinMaxSizeType.None;
        /// <summary> The min height type of the object. </summary>
        public MinMaxSizeType MinHeightType
        {
            get { return this._minHeightType; }
            set {
                this._minHeightType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _minHeight = 0;
        /// <summary> The min fixed height of the object. </summary>
        public float MinHeight
        {
            get { return this._minHeight; }
            set {
                this._minHeight = Mathf.Max(value, 0);
                this._minHeightType = MinMaxSizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _minHeightOfParent = 0;
        /// <summary> The min relative height of the object. </summary>
        public float MinHeightOfParent
        {
            get { return this._minHeightOfParent; }
            set {
                this._minHeightOfParent = Mathf.Max(value, 0);
                this._minHeightType = MinMaxSizeType.Fill;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private MinMaxSizeType _minDepthType = MinMaxSizeType.None;
        /// <summary> The min depth type of the object. </summary>
        public MinMaxSizeType MinDepthType
        {
            get { return this._minDepthType; }
            set {
                this._minDepthType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _minDepth = 0;
        /// <summary> The min fixed depth of the object. </summary>
        public float MinDepth
        {
            get { return this._minDepth; }
            set {
                this._minDepth = Mathf.Max(value, 0);
                this._minDepthType = MinMaxSizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _minDepthOfParent = 0;
        /// <summary> The min relative depth of the object. </summary>
        public float MinDepthOfParent
        {
            get { return this._minDepthOfParent; }
            set {
                this._minDepthOfParent = Mathf.Max(value, 0);
                this._minDepthType = MinMaxSizeType.Fill;
                this.MarkDirty();
            }
        }

        /// <summary> The max fixed size of the object. </summary>
        public Vector3 MaxSize
        {
            get => new Vector3(this._maxWidth, this._maxHeight, this._maxDepth);
            set
            {
                this.MaxWidth = value.x;
                this.MaxHeight = value.y;
                this.MaxDepth = value.z;
            }
        }

        /// <summary> The max relative size of the object. </summary>
        public Vector3 MaxSizeOfParent
        {
            get => new Vector3(this._maxWidthOfParent, this._maxHeightOfParent, this._maxDepthOfParent);
            set
            {
                this.MaxWidthOfParent = value.x;
                this.MaxHeightOfParent = value.y;
                this.MaxDepthOfParent = value.z;
            }
        }

        [SerializeField]
        private MinMaxSizeType _maxWidthType = MinMaxSizeType.None;
        /// <summary> The max width type of the object. </summary>
        public MinMaxSizeType MaxWidthType
        {
            get { return this._maxWidthType; }
            set {
                this._maxWidthType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _maxWidth = 1;
        /// <summary> The max fixed max width of the object. </summary>
        public float MaxWidth
        {
            get { return this._maxWidth; }
            set {
                this._maxWidth = Mathf.Max(value, 0);
                this._maxWidthType = MinMaxSizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _maxWidthOfParent = 1;
        /// <summary> The max relative width of the object. </summary>
        public float MaxWidthOfParent
        {
            get { return this._maxWidthOfParent; }
            set {
                this._maxWidthOfParent = Mathf.Max(value, 0);
                this._maxWidthType = MinMaxSizeType.Fill;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private MinMaxSizeType _maxHeightType = MinMaxSizeType.None;
        /// <summary> The max height type of the object. </summary>
        public MinMaxSizeType MaxHeightType
        {
            get { return this._maxHeightType; }
            set {
                this._maxHeightType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _maxHeight = 1;
        /// <summary> The max fixed height of the object. </summary>
        public float MaxHeight
        {
            get { return this._maxHeight; }
            set {
                this._maxHeight = Mathf.Max(value, 0);
                this._maxHeightType = MinMaxSizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _maxHeightOfParent = 1;
        /// <summary> The max relative height of the object. </summary>
        public float MaxHeightOfParent
        {
            get { return this._maxHeightOfParent; }
            set {
                this._maxHeightOfParent = Mathf.Max(value, 0);
                this._maxHeightType = MinMaxSizeType.Fill;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private MinMaxSizeType _maxDepthType = MinMaxSizeType.None;
        /// <summary> The max depth type of the object. </summary>
        public MinMaxSizeType MaxDepthType
        {
            get { return this._maxDepthType; }
            set {
                this._maxDepthType = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _maxDepth = 1;
        /// <summary> The max fixed depth of the object. </summary>
        public float MaxDepth
        {
            get { return this._maxDepth; }
            set {
                this._maxDepth = Mathf.Max(value, 0);
                this._maxDepthType = MinMaxSizeType.Fixed;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _maxDepthOfParent = 1;
        /// <summary> The max relative depth of the object. </summary>
        public float MaxDepthOfParent
        {
            get { return this._maxDepthOfParent; }
            set {
                this._maxDepthOfParent = Mathf.Max(value, 0);
                this._maxDepthType = MinMaxSizeType.Fill;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private Vector3 _offset = Vector3.zero;
        /// <summary> Use offset to add an offset to the final position of the gameObject after layout is complete. </summary>
        public Vector3 Offset
        {
            get { return this._offset; }
            set { this._offset = value; this.MarkDirty(); }
        }

        [SerializeField]
        private Vector3 _scale = Vector3.one;
        /// <summary> Use rotation to scale the size of the gameObject before layout runs.
        /// This will generate a new size to encapsulate the scaled object. </summary>
        public Vector3 Scale
        {
            get { return this._scale; }
            set { this._scale = value; this.MarkDirty(); }
        }

        [SerializeField]
        private Quaternion _rotation = Quaternion.identity;
        /// <summary> Use rotation to set the rotation of the gameObject before layout runs.
        /// This will generate a new size to encapsulate the rotated object. </summary>
        public Quaternion Rotation
        {
            get { return this._rotation; }
            set { this._rotation = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _marginLeft;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginLeft
        {
            get { return this._marginLeft; }
            set { this._marginLeft = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _marginRight;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginRight
        {
            get { return this._marginRight; }
            set { this._marginRight = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _marginTop;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginTop
        {
            get { return this._marginTop; }
            set { this._marginTop = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _marginBottom;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginBottom
        {
            get { return this._marginBottom; }
            set { this._marginBottom = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _marginFront;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginFront
        {
            get { return this._marginFront; }
            set { this._marginFront = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _marginBack;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginBack
        {
            get { return this._marginBack; }
            set { this._marginBack = value; this.MarkDirty(); }
        }

        /// <summary> Margin to add additional space around a gameObject. </summary>
        public Directions Margin
        {
            get => new Directions(new float[] {
                this._marginRight, this._marginLeft, this._marginTop, this._marginBottom, this._marginBack, this._marginFront});
            set
            {
                this._marginRight = value[0];
                this._marginLeft = value[1];
                this._marginTop = value[2];
                this._marginBottom = value[3];
                this._marginBack = value[4];
                this._marginFront = value[5];
                this.MarkDirty();
            }
        }

        [SerializeField]
        private float _paddingLeft;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingLeft
        {
            get { return this._paddingLeft; }
            set { this._paddingLeft = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _paddingRight;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingRight
        {
            get { return this._paddingRight; }
            set { this._paddingRight = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _paddingTop;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingTop
        {
            get { return this._paddingTop; }
            set { this._paddingTop = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _paddingBottom;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingBottom
        {
            get { return this._paddingBottom; }
            set { this._paddingBottom = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _paddingFront;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingFront
        {
            get { return this._paddingFront; }
            set { this._paddingFront = value; this.MarkDirty(); }
        }

        [SerializeField]
        private float _paddingBack;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingBack
        {
            get { return this._paddingBack; }
            set { this._paddingBack = value; this.MarkDirty(); }
        }

        /// <summary> Padding to reduce available space inside a layout. </summary>
        public Directions Padding
        {
            get => new Directions(new float[] {
                this._paddingRight, this._paddingLeft, this._paddingTop, this._paddingBottom, this._paddingBack, this._paddingFront});
            set
            {
                this._paddingRight = value[0];
                this._paddingLeft = value[1];
                this._paddingTop = value[2];
                this._paddingBottom = value[3];
                this._paddingBack = value[4];
                this._paddingFront = value[5];
                this.MarkDirty();
            }
        }

        [SerializeField]
        private bool _skipLayout;
        /// <summary> Skip layout for this object. </summary>
        public bool SkipLayout
        {
            get => this._skipLayout;
            set
            {
                this._skipLayout = value;
                this.MarkDirty();
            }
        }

        [SerializeField]
        private bool _useDefaultAdapter = true;
        /// <summary>
        /// The 'adapter' on a Flexalon Object is used to control how the object is measured and scaled.
        /// If you don't supply a custom adapter, then Flexalon will use a default adapter for common
        /// Unity components like MeshRenderer, TextMeshPro, etc. Use this property to disable that adapter
        /// and treat this Flexalon Object as an empty gameObject.
        /// </summary>
        public bool UseDefaultAdapter
        {
            get => this._useDefaultAdapter;
            set
            {
                this._useDefaultAdapter = value;
                this.MarkDirty();
            }
        }

        /// <inheritdoc />
        protected override void ResetProperties()
        {
            this._node.SetFlexalonObject(null);
        }

        /// <inheritdoc />
        protected override void UpdateProperties()
        {
            this._node.SetFlexalonObject(this);
        }

#if false
        private Transform _lastParent;

        /// <inheritdoc />
        public override void DoUpdate()
        {
            if (Application.isPlaying || Node.Dirty)
            {
                return;
            }

            // Don't update prefab instances
            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                return;
            }

            var result = _node.Result;

            // Don't do any of this if the parent changed.
            if (_lastParent != transform.parent)
            {
                _lastParent = transform.parent;
                result.TargetScale = transform.localScale;
                result.TransformScale = transform.localScale;
                result.TargetRotation = transform.localRotation;
                result.TransformRotation = transform.localRotation;
                result.TargetPosition = transform.localPosition;
                result.TransformPosition = transform.localPosition;
            }

            // Detect changes to the object's position, rotation, scale, and rect size which may happen
            // when the developer uses the transform control, enters new values in the
            // inspector, or various other scenarios. Maintain those edits
            // by modifying the offset, rotation, and scale on the FlexalonObject.

            bool isRectTransform = false;
            if (transform is RectTransform rectTransform)
            {
                if (_widthType == SizeType.Fixed)
                {
                    if (rectTransform.rect.size.x != _width)
                    {
                        // Avoid recording changes here to avoid screen size changes causing edits.
                        _width = rectTransform.rect.size.x;
                    }
                }

                if (_heightType == SizeType.Fixed)
                {
                    if (rectTransform.rect.size.y != _height)
                    {
                        // Avoid recording changes here to avoid screen size changes causing edits.
                        _height = rectTransform.rect.size.y;
                    }
                }

                isRectTransform = true;
            }

            bool shouldScale = _node.Adapter.TryGetScale(_node, out var s);
            if (shouldScale && result.TransformScale != transform.localScale)
            {
                UnityEditor.Undo.RecordObject(this, "Scale change");
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                UnityEditor.Undo.RecordObject(result, "Scale change");
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                Flexalon.RecordFrameChanges = true;
                _scale = Math.Mul(Scale, Math.Div(transform.localScale, result.TransformScale));
                if (float.IsNaN(_scale.x) || Mathf.Abs(_scale.x) < 1e-5f) _scale.x = 0;
                if (float.IsNaN(_scale.y) || Mathf.Abs(_scale.y) < 1e-5f) _scale.y = 0;
                if (float.IsNaN(_scale.z) || Mathf.Abs(_scale.z) < 1e-5f) _scale.z = 0;
                if (Mathf.Abs(1f - _scale.x) < 1e-5f) _scale.x = 1;
                if (Mathf.Abs(1f - _scale.y) < 1e-5f) _scale.y = 1;
                if (Mathf.Abs(1f - _scale.z) < 1e-5f) _scale.z = 1;
                result.TargetScale = transform.localScale;
                result.TransformScale = transform.localScale;
                _node.Parent?.MarkDirty();

                if (_node.Constraint != null)
                {
                    _node.MarkDirty();
                }
                else
                {
                    _node.ApplyScaleAndRotation();
                }

                // The scale and rect transform controls affect both position and scale,
                // That's not expected in a layout, so early out here to avoid setting the position.
                return;
            }

            bool inLayoutOrConstraint =
                (_node.Parent != null && !_node.Parent.Dirty && transform.parent == _node.Parent.GameObject.transform) ||
                (_node.Constraint != null && _node.Constraint.Target != null);

            if (inLayoutOrConstraint)
            {
                if (!isRectTransform && result.TransformPosition != transform.localPosition)
                {
                    UnityEditor.Undo.RecordObject(this, "Offset change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                    UnityEditor.Undo.RecordObject(result, "Offset change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);

                    if (transform is RectTransform offsetRectTransform)
                    {
                    }

                    if (Node.Constraint != null && Node.Constraint.Target != null)
                    {
                        _offset += Quaternion.Inverse(Node.Constraint.Target.transform.rotation) * (transform.localPosition - result.TransformPosition);
                    }
                    else
                    {
                        _offset += Math.Mul(Node.Parent.Result?.ComponentScale ?? Vector3.one, (transform.localPosition - result.TransformPosition));
                    }

                    if (float.IsNaN(_offset.x) || Mathf.Abs(_offset.x) < 1e-5f) _offset.x = 0;
                    if (float.IsNaN(_offset.y) || Mathf.Abs(_offset.y) < 1e-5f) _offset.y = 0;
                    if (float.IsNaN(_offset.z) || Mathf.Abs(_offset.z) < 1e-5f) _offset.z = 0;

                    result.TargetPosition = transform.localPosition;
                    result.TransformPosition = transform.localPosition;
                }

                if (result.TransformRotation != transform.localRotation)
                {
                    UnityEditor.Undo.RecordObject(this, "Rotation change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                    UnityEditor.Undo.RecordObject(result, "Rotation change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                    Flexalon.RecordFrameChanges = true;

                    if (Node.Constraint != null && Node.Constraint.Target != null)
                    {
                        _rotation = Quaternion.Inverse(Node.Constraint.Target.transform.rotation) * transform.rotation;
                    }
                    else
                    {
                        _rotation *= transform.localRotation * Quaternion.Inverse(result.TransformRotation);
                    }

                    if (float.IsNaN(_rotation.x) || Mathf.Abs(_rotation.x) < 1e-5f) _rotation.x = 0;
                    if (float.IsNaN(_rotation.y) || Mathf.Abs(_rotation.y) < 1e-5f) _rotation.y = 0;
                    if (float.IsNaN(_rotation.z) || Mathf.Abs(_rotation.z) < 1e-5f) _rotation.z = 0;
                    if (float.IsNaN(_rotation.w) || Mathf.Abs(1 - _rotation.w) < 1e-5f) _rotation.w = 1;

                    _rotation.Normalize();
                    result.TargetRotation = transform.localRotation;
                    result.TransformRotation = transform.localRotation;
                    _node.Parent?.MarkDirty();

                    if (_node.Constraint != null)
                    {
                        _node.MarkDirty();
                    }
                    else
                    {
                        _node.ApplyScaleAndRotation();
                    }
                }
            }
            else
            {
                if (result.TransformRotation != transform.localRotation)
                {
                    UnityEditor.Undo.RecordObject(result, "Rotation change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                    result.TargetRotation = transform.localRotation;
                    result.TransformRotation = transform.localRotation;
                    _node.ApplyScaleAndRotation();
                }
            }
        }
#endif

        protected override void Initialize()
        {
            base.Initialize();
            if (this.transform is RectTransform || (this.transform.parent && this.transform.parent is RectTransform))
            {
                this._width = 100;
                this._height = 100;
                this._maxWidth = 100;
                this._maxHeight = 100;
            }
        }

        protected override void Upgrade(int fromVersion)
        {
#if UNITY_UI
            // UPGRADE FIX: In v4.0 canvas no longer scales to fit layout size.
            // Instead, scale needs to be set on the FlexalonObject.
            if (fromVersion < 4 && this.TryGetComponent<Canvas>(out var canvas))
            {
                this._widthType = SizeType.Component;
                this._heightType = SizeType.Component;

                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    this._scale = canvas.transform.localScale;
                    this._node.Result.AdapterBounds = new Bounds(Vector3.zero, (this.transform as RectTransform).rect.size);
                }
            }
#endif
        }
    }
}