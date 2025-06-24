using System;
using System.Collections.Generic;
using Flexalon.Runtime.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Flexalon.Runtime.Interaction
{
    /// <summary> Allows a gameObject to be clicked and dragged. </summary>
    [AddComponentMenu("Flexalon/Flexalon Interactable"), HelpURL("https://www.flexalon.com/docs/interactable"), DisallowMultipleComponent]
    public class FlexalonInteractable : MonoBehaviour
    {
        [SerializeField]
        private bool _clickable = false;
        /// <summary> Determines if this object can be clicked and generate click events. </summary>
        public bool Clickable {
            get => this._clickable;
            set => this._clickable = value;
        }

        [SerializeField]
        private float _maxClickTime = 0.1f;
        /// <summary>
        /// With a mouse or touch input, a click is defined as a press and release.
        /// The time between press and release must be less than Max Click Time to
        /// count as a click. A drag interaction cannot start until Max Click Time is exceeded.
        /// </summary>
        public float MaxClickTime {
            get => this._maxClickTime;
            set => this._maxClickTime = value;
        }

        [SerializeField]
        private float _maxClickDistance = 0.1f;
        /// <summary>
        /// With a mouse or touch input, a click is defined as a press and release.
        /// The distance between press and release must be less than Max Click Distance to
        /// count as a click. Otherwise, the interaction is considered a drag.
        /// </summary>
        public float MaxClickDistance {
            get => this._maxClickDistance;
            set => this._maxClickDistance = value;
        }

        [SerializeField]
        private bool _draggable = false;
        /// <summary> Determines if this object can be dragged and generate drag events. </summary>
        public bool Draggable {
            get => this._draggable;
            set => this._draggable = value;
        }

        [SerializeField]
        private float _interpolationSpeed = 10;
        /// <summary> How quickly the object moves towards the cursor when dragged. </summary>
        public float InterpolationSpeed {
            get => this._interpolationSpeed;
            set => this._interpolationSpeed = value;
        }

        [SerializeField]
        private float _insertRadius = 0.5f;
        /// <summary> How close this object needs to a drag target's bounds to be inserted. </summary>
        public float InsertRadius {
            get => this._insertRadius;
            set => this._insertRadius = value;
        }

        /// <summary> Restricts the movement of an object during a drag. </summary>
        public enum RestrictionType
        {
            /// <summary> No restriction ensures the object can move freely. </summary>
            None,

            /// <summary> Plane restriction ensures the object moves along a plane, defined
            /// by the objects initial position and the Plane Normal property. </summary>
            Plane,

            /// <summary> Line restriction ensures the object moves along a line, defined
            /// by the object's initial position and the Line Direction property. </summary>
            Line
        }

        [SerializeField]
        private RestrictionType _restriction = RestrictionType.None;
        /// <summary> Determines how to restrict the object's drag movement. </summary>
        public RestrictionType Restriction {
            get => this._restriction;
            set => this._restriction = value;
        }

        [SerializeField]
        private Vector3 _planeNormal = Vector3.up;
        /// <summary> Defines the normal of the plane when using a plane restriction.
        /// If 'Local Space' is checked, this normal is rotated by the transform
        /// of the layout that the object started in. </summary>
        public Vector3 PlaneNormal {
            get => this._planeNormal;
            set
            {
                this._restriction = RestrictionType.Plane;
                this._planeNormal = value;
            }
        }

        [SerializeField]
        private Vector3 _lineDirection = Vector3.right;
        /// <summary> Defines the direction of the line when using a line restriction.
        /// If 'Local Space'is checked, this direction is rotated by the transform
        /// of the layout that the object started in. </summary>
        public Vector3 LineDirection {
            get => this._lineDirection;
            set
            {
                this._restriction = RestrictionType.Line;
                this._lineDirection = value;
            }
        }

        [SerializeField]
        private bool _localSpaceRestriction = true;
        /// <summary> When checked, the Plane Normal and Line Direction are applied in local space. </summary>
        public bool LocalSpaceRestriction {
            get => this._localSpaceRestriction;
            set => this._localSpaceRestriction = value;
        }

        [SerializeField]
        private Vector3 _holdOffset;
        // <summary> When dragged, this option adds an offset to the dragged object's position.
        // This can be used to float the object near the layout while it is being dragged.
        // If 'Local Space' is checked, this offset is rotated and scaled by the transform
        // of the layout that the object started in. </summary>
        public Vector3 HoldOffset {
            get => this._holdOffset;
            set => this._holdOffset = value;
        }

        [SerializeField]
        private bool _localSpaceOffset = true;
        /// <summary> When checked, the Hold Offset is applied in local space. </summary>
        public bool LocalSpaceOffset {
            get => this._localSpaceOffset;
            set => this._localSpaceOffset = value;
        }

        [SerializeField]
        private bool _rotateOnDrag = false;
        // <summary> When dragged, this option adds a rotation to the dragged object.
        // This can be used to tilt the object while it is being dragged.
        // If 'Local Space' is checked, this rotation will be in the local
        // space of the layout that the object started in. </summary>
        public bool RotateOnDrag {
            get => this._rotateOnDrag;
            set => this._rotateOnDrag = value;
        }

        [SerializeField]
        private Quaternion _holdRotation;
        /// <summary> The rotation to apply to the object when it is being dragged. </summary>
        public Quaternion HoldRotation {
            get => this._holdRotation;
            set
            {
                this._rotateOnDrag = true;
                this._holdRotation = value;
            }
        }

        [SerializeField]
        private bool _localSpaceRotation = true;
        /// <summary> When checked, the Hold Rotation is applied in local space. </summary>
        public bool LocalSpaceRotation {
            get => this._localSpaceRotation;
            set => this._localSpaceRotation = value;
        }

        [SerializeField]
        private bool _hideCursor = false;
        /// <summary> When checked, Cursor.visible is set to false when the object is dragged. </summary>
        public bool HideCursor {
            get => this._hideCursor;
            set => this._hideCursor = value;
        }

        [SerializeField]
        private GameObject _handle = null;
        /// <summary> GameObject to use to select and drag this object. If not set, uses self. </summary>
        public GameObject Handle {
            get => this._handle;
            set
            {
                FlexalonInteractable._raycaster.Unregister(this);
                this._handle = value;
                FlexalonInteractable._raycaster.Register(this);
            }
        }

#if UNITY_PHYSICS
        [SerializeField, Obsolete("Use Handle instead.")]
        private Collider _collider;

        void OnValidate()
        {
            #pragma warning disable 618
            if (this._collider && !this._handle)
            {
                this._handle = this._collider.gameObject;
                this._collider = null;
            }
            #pragma warning restore 618
        }

        [SerializeField]
        private Collider _bounds;
        /// <summary> If set, the object cannot be dragged outside of the bounds collider. </summary>
        public Collider Bounds {
            get => this._bounds;
            set => this._bounds = value;
        }
#endif

        [SerializeField]
        private LayerMask _layerMask = -1;
        /// <summary> When dragged, limits which Flexalon Drag Targets will accept this object
        /// by comparing the Layer Mask to the target GameObject's layer. </summary>
        public LayerMask LayerMask {
            get => this._layerMask;
            set => this._layerMask = value;
        }

        /// <summary> An event that occurs to a FlexalonInteractable. </summary>
        [System.Serializable]
        public class InteractableEvent : UnityEvent<FlexalonInteractable>{}

        [SerializeField]
        private InteractableEvent _clicked;
        /// <summary> Unity Event invoked when the object is pressed and released within MaxClickTime. </summary>
        public InteractableEvent Clicked => this._clicked;

        [SerializeField]
        private InteractableEvent _hoverStart;
        /// <summary> Unity Event invoked when the object starts being hovered. </summary>
        public InteractableEvent HoverStart => this._hoverStart;

        [SerializeField]
        private InteractableEvent _hoverEnd;
        /// <summary> Unity Event invoked when the object stops being hovered. </summary>
        public InteractableEvent HoverEnd => this._hoverEnd;

        [SerializeField]
        private InteractableEvent _selectStart;
        /// <summary> Unity Event invoked when the object starts being selected (e.g. press down mouse over object). </summary>
        public InteractableEvent SelectStart => this._selectStart;

        [SerializeField]
        private InteractableEvent _selectEnd;
        /// <summary> Unity Event invoked when the object stops being selected (e.g. release mouse). </summary>
        public InteractableEvent SelectEnd => this._selectEnd;

        [SerializeField]
        private InteractableEvent _dragStart;
        /// <summary> Unity Event invoked when the object starts being dragged. </summary>
        public InteractableEvent DragStart => this._dragStart;

        [SerializeField]
        private InteractableEvent _dragEnd;
        /// <summary> Unity Event invoked when the object stops being dragged. </summary>
        public InteractableEvent DragEnd => this._dragEnd;

        [SerializeField]
        private InteractableEvent _dragTragetChanged;
        /// <summary> Event when the drag target or sibling index changes during a drag operation </summary>
        public InteractableEvent DragTargetChanged => this._dragTragetChanged;

        private static List<FlexalonInteractable> _hoveredObjects = new List<FlexalonInteractable>();
        /// <summary> The currently hovered objects. </summary>
        public static List<FlexalonInteractable> HoveredObjects => FlexalonInteractable._hoveredObjects;

        /// <summary> The first hovered object. </summary>
        public static FlexalonInteractable HoveredObject => FlexalonInteractable._hoveredObjects.Count > 0 ? FlexalonInteractable._hoveredObjects[0] : null;

        private static List<FlexalonInteractable> _selectedObjects = new List<FlexalonInteractable>();
        /// <summary> The currently selected / dragged objects. </summary>
        public static List<FlexalonInteractable> SelectedObjects => FlexalonInteractable._selectedObjects;

        /// <summary> The first selected / dragged object. </summary>
        public static FlexalonInteractable SelectedObject => FlexalonInteractable._selectedObjects.Count > 0 ? FlexalonInteractable._selectedObjects[0] : null;

        private Vector3 _target;
        private Vector3 _lastTarget;
        private float _distance;
        private GameObject _placeholder;
        private Vector3 _startPosition;
        private int _startSiblingIndex;
        private UnityEngine.Plane _plane = new UnityEngine.Plane();
        private static FlexalonRaycaster _raycaster = new FlexalonRaycaster();
        private Transform _localSpace;
        private Transform _lastValidLocalSpace;
        private float _selectTime;
        private Vector3 _selectPosition;
        private Vector3 _clickOffset;
        private InputProvider _inputProvider;
        private FlexalonNode _node;
        private bool _wasActive;

#if UNITY_UI
        private Canvas _canvas;
        internal Canvas Canvas => this._canvas;
#endif

        // For Editor
        internal bool _showAllDragProperties => this.GetInputProvider().InputMode == InputMode.Raycast;

        /// <summary> The current state of the interactable. </summary>
        public enum InteractableState
        {
            /// <summary> The object is not being interacted with. </summary>
            Init,

            /// <summary> The object is being hovered over. </summary>
            Hovering,

            /// <summary> The object is being selected (e.g. press down mouse over object). </summary>
            Selecting,

            /// <summary> The object is being dragged. </summary>
            Dragging
        }

        private InteractableState _state = InteractableState.Init;
        /// <summary> The current state of the interactable. </summary>
        public InteractableState State => this._state;

        /// <summary> The drag target that will be attached if the dragged object is released. </summary>
        public Transform DragTarget => this._placeholder != null ? this._placeholder.transform.parent : null;

        /// <summary> The sibling index that this object will be inserted into the drag target. </summary>
        public int DragSiblingIndex => this._placeholder != null ? this._placeholder.transform.GetSiblingIndex() : 0;

        void Awake()
        {
            if (this._clicked == null)
            {
                this._clicked = new InteractableEvent();
            }

            if (this._hoverStart == null)
            {
                this._hoverStart = new InteractableEvent();
            }

            if (this._hoverEnd == null)
            {
                this._hoverEnd = new InteractableEvent();
            }

            if (this._selectStart == null)
            {
                this._selectStart = new InteractableEvent();
            }

            if (this._selectEnd == null)
            {
                this._selectEnd = new InteractableEvent();
            }

            if (this._dragStart == null)
            {
                this._dragStart = new InteractableEvent();
            }

            if (this._dragEnd == null)
            {
                this._dragEnd = new InteractableEvent();
            }
        }

        void OnEnable()
        {
            this._node = Core.Flexalon.GetOrCreateNode(this.gameObject);
            this._inputProvider = this.GetInputProvider();

            this.UpdateCanvas();

            if (!this._handle)
            {
                this._handle = this.gameObject;
            }

            FlexalonInteractable._raycaster.Register(this);
        }

        void OnDisable()
        {
            FlexalonInteractable._raycaster.Unregister(this);
            if (this._state != InteractableState.Init)
            {
                this.UpdateState(this._inputProvider.InputMode, default, false, false, false);
            }

            this._node = null;
        }

        void Update()
        {
            var inputMode = this._inputProvider.InputMode;
            Vector3 uiPointer = this._inputProvider.UIPointer;
            Ray ray = this._inputProvider.Ray;
            bool isHit = false;
            bool isActive = this._inputProvider.Active;
            bool becameActive = isActive && !this._wasActive;
            this._wasActive = isActive;

            if (inputMode == InputMode.Raycast)
            {
                if (FlexalonInteractable._selectedObjects.Count == 0 || FlexalonInteractable._selectedObjects[0] == this)
                {
                    isHit = FlexalonInteractable._raycaster.IsHit(uiPointer, ray, this);
                }
            }
            else
            {
                var focusedObject = this._inputProvider.ExternalFocusedObject;
                isHit = focusedObject && focusedObject == this.gameObject;
            }

#if UNITY_UI
            if (this._canvas && this._canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                ray = new Ray(uiPointer, Vector3.forward);
            }
#endif

            this.UpdateState(inputMode, ray, isHit, isActive, becameActive);
        }

        void FixedUpdate()
        {
            if (this._state != InteractableState.Dragging)
            {
                return;
            }

            if (this._target == this._lastTarget)
            {
                return;
            }

            var currentDragTarget = this._placeholder.transform.parent ? this._placeholder.transform.parent.GetComponent<FlexalonDragTarget>() : null;
            bool dragTargetChanged = false;

            // Find a drag target to insert into.
            if (this.TryFindNearestDragTarget(currentDragTarget, out var newDragTarget, out var nearestChild))
            {
                dragTargetChanged = this.AddToLayout(currentDragTarget, newDragTarget, nearestChild);
            }
            else
            {
                dragTargetChanged = this.MovePlaceholder(null);
            }

            this._lastTarget = this._target;

            if (dragTargetChanged)
            {
                this.DragTargetChanged.Invoke(this);
            }
        }

        internal void UpdateCanvas()
        {
#if UNITY_UI
            if (this._canvas)
            {
                return;
            }

            this._canvas = this.GetComponentInParent<Canvas>();

            if (this._canvas)
            {
                if (this._restriction == RestrictionType.None)
                {
                    this._restriction = RestrictionType.Plane;
                    this._planeNormal = Vector3.forward;
                }
            }
#endif

        }

        private InputProvider GetInputProvider()
        {
            var inputProvider = this.GetComponent<InputProvider>();
            if (inputProvider == null)
            {
                inputProvider = Core.Flexalon.GetInputProvider();
            }

            return inputProvider;
        }

        private void SetState(InteractableState state)
        {
            this._state = state;
        }

        private void UpdateState(InputMode inputMode, Ray ray, bool isHit, bool isActive, bool becameActive)
        {
            if (this._state == InteractableState.Init)
            {
                if (isHit && (!isActive || becameActive))
                {
                    this.SetState(InteractableState.Hovering);
                    this.OnHoverStart();
                }
            }

            if (this._state == InteractableState.Hovering)
            {
                if (!isHit)
                {
                    this.SetState(InteractableState.Init);
                    this.OnHoverEnd();
                }
                else if (becameActive)
                {
                    this.SetState(InteractableState.Selecting);
                    this.OnSelectStart();
                }
            }

            if (this._state == InteractableState.Selecting)
            {
                bool clickValid = this._clickable && isHit &&
                    (Time.time - this._selectTime <= this._maxClickTime) &&
                    Vector3.Distance(this._selectPosition, FlexalonInteractable._raycaster.hitPosition) < this._maxClickDistance;

                if (!isActive)
                {
                    if (clickValid)
                    {
                        this.Clicked.Invoke(this);
                    }

                    if (isHit)
                    {
                        this.SetState(InteractableState.Hovering);
                        this.OnSelectEnd();
                    }
                    else
                    {
                        this.SetState(InteractableState.Init);
                        this.OnSelectEnd();
                        this.OnHoverEnd();
                    }

                }
                else if (this._draggable && !clickValid)
                {
                    this.SetState(InteractableState.Dragging);
                    this.OnDragStart(inputMode, ray);
                }
            }

            if (this._state == InteractableState.Dragging)
            {
                if (!isActive)
                {
                    if (isHit)
                    {
                        this.SetState(InteractableState.Hovering);
                        this.OnDragEnd();
                        this.OnSelectEnd();
                    }
                    else
                    {
                        this.SetState(InteractableState.Init);
                        this.OnDragEnd();
                        this.OnSelectEnd();
                        this.OnHoverEnd();
                    }
                }
                else
                {
                    this.OnDragMove(inputMode, ray);
                }
            }
        }

        private void OnHoverStart()
        {
            FlexalonInteractable._hoveredObjects.Add(this);
            this.HoverStart.Invoke(this);

            // Save this here in case the input provider changes the parent on select.
            this._localSpace = this.transform.parent;
            this._startSiblingIndex = this.transform.GetSiblingIndex();
        }

        private void OnHoverEnd()
        {
            FlexalonInteractable._hoveredObjects.Remove(this);
            this.HoverEnd.Invoke(this);
            this._localSpace = null;
        }

        private void OnSelectStart()
        {
            this._selectTime = Time.time;
            this._selectPosition = FlexalonInteractable._raycaster.hitPosition;
            FlexalonInteractable._selectedObjects.Add(this);
            this.SelectStart.Invoke(this);
        }

        private void OnSelectEnd()
        {
            FlexalonInteractable._selectedObjects.Remove(this);
            this.SelectEnd.Invoke(this);
        }

        private void OnDragStart(InputMode inputMode, Ray ray)
        {
            if (this._hideCursor)
            {
                Cursor.visible = false;
            }

            this._target = this._lastTarget = this.transform.position;
            this._clickOffset = this.transform.position - this._selectPosition;
            this._distance = Vector3.Distance(this._target, ray.origin + this._clickOffset);
            this._startPosition = this.transform.position;

            // Create a placeholder
            this._placeholder = new GameObject("Drag Placeholder");
            var placeholderObj = Core.Flexalon.AddComponent<FlexalonObject>(this._placeholder);
            this._node = Core.Flexalon.GetOrCreateNode(this.gameObject);
            placeholderObj.Size = this._node.Result.LayoutBounds.size;
            placeholderObj.Rotation = this._node.Rotation;
            placeholderObj.Scale = this._node.Scale;
            placeholderObj.Margin = this._node.Margin;
            placeholderObj.Padding = this._node.Padding;

            this._node.IsDragging = true;

            // If we're in a valid drag target, swap with the placeholder.
            var parentDragTarget = this._localSpace ? this._localSpace.GetComponent<FlexalonDragTarget>() : null;
            if (parentDragTarget != null && this.CanAdd(parentDragTarget, parentDragTarget))
            {
                this.MovePlaceholder(this._localSpace, this._startSiblingIndex);

                // Input provider may be changing the parent before we get here.
                if (this.transform.parent == this._localSpace)
                {
#if UNITY_UI
                    this.transform.SetParent(this._canvas?.transform, true);
#else
                    transform.SetParent(null, true);
#endif
                }
            }
            else
            {
                this._placeholder.transform.SetParent(null);
                this._placeholder.SetActive(false);
            }

            this.DragStart.Invoke(this);
            this.DragTargetChanged.Invoke(this);
        }

        private void OnDragMove(InputMode inputMode, Ray ray)
        {
            if (inputMode == InputMode.External)
            {
                this._target = this.transform.position;
            }
            else
            {
                this.UpdateTarget(ray);
                this.UpdateObjectPosition();
            }
        }

        private void OnDragEnd()
        {
            this._node.IsDragging = false;

            // Swap places with the placeholder and destroy it.
            if (this._placeholder.activeSelf)
            {
                this.transform.SetParent(this._placeholder.transform.parent, true);
                this.transform.SetSiblingIndex(this._placeholder.transform.GetSiblingIndex());
            }

            this._lastValidLocalSpace = null;
            this._placeholder.transform.SetParent(null);
            Destroy(this._placeholder);

            if (this._hideCursor)
            {
                Cursor.visible = true;
            }

            this.DragEnd.Invoke(this);
        }

        private static bool ClosestPointOnTwoLines(Vector3 p0, Vector3 v0, Vector3 p1, Vector3 v1, out Vector3 closestPointLine2)
        {
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(v0, v0);
            float b = Vector3.Dot(v0, v1);
            float e = Vector3.Dot(v1, v1);

            float d = a * e - b * b;

            // Lines are not parallel
            if (d != 0.0f)
            {
                Vector3 r = p0 - p1;
                float c = Vector3.Dot(v0, r);
                float f = Vector3.Dot(v1, r);
                float t = (a * f - c * b) / d;
                closestPointLine2 = p1 + v1 * t;
                return true;
            }

            return false;
        }

        // Sets _target to where we want to move the dragged object -- based on the input ray, restrictions, and bounds.
        private void UpdateTarget(Ray ray)
        {
            ray.origin += this._clickOffset;

            if (this._restriction == RestrictionType.Line)
            {
                var lineDir = this._lineDirection;
                if (this._localSpaceRestriction && this._lastValidLocalSpace)
                {
                    lineDir = this._lastValidLocalSpace.rotation * this._lineDirection;
                }

                if (!FlexalonInteractable.ClosestPointOnTwoLines(ray.origin, ray.direction, this._startPosition, lineDir.normalized, out this._target))
                {
                    this._target = this._startPosition;
                }
            }
            else if (this._restriction == RestrictionType.Plane)
            {
                var normal = this._planeNormal;
                if (this._localSpaceRestriction && this._lastValidLocalSpace)
                {
                    normal = this._lastValidLocalSpace.rotation * this._planeNormal;
                }

                this._plane.SetNormalAndPosition(normal.normalized, this._startPosition);
                this._plane.Raycast(ray, out var distance);
                this._target = ray.origin + ray.direction * distance;
            }
            else
            {
                // If there's no restriction, just project forward at the same distance as the placeholder.
                if (this._placeholder.gameObject.activeSelf && Core.Flexalon.GetOrCreateNode(this._placeholder).HasResult)
                {
                    this._distance = Vector3.Distance(ray.origin, this._placeholder.transform.position);
                }

                this._target = ray.origin + ray.direction * this._distance;
            }

#if UNITY_PHYSICS
            // Apply bounds restriction
            if (this._bounds)
            {
                this._target = this._bounds.ClosestPoint(this._target);
            }
#endif
        }

        private void UpdateObjectPosition()
        {
            // Apply hold offset
            var offset = Vector3.zero;
            if (this._localSpaceOffset && this._localSpace)
            {
                offset = this._localSpace.localToWorldMatrix.MultiplyVector(this._holdOffset);
            }
            else if (!this._localSpaceOffset)
            {
                offset = this._holdOffset;
            }

            // Interpolate object towards target.
            this.transform.position = Vector3.Lerp(this.transform.position, this._target + offset, Time.deltaTime * this._interpolationSpeed);

            // Apply hold rotation
            if (this._rotateOnDrag)
            {
                var rotation = Quaternion.identity;
                if (this._localSpaceRotation && this._localSpace)
                {
                    rotation = this._localSpace.rotation * this._holdRotation;
                }
                else if (!this._localSpaceRotation)
                {
                    rotation = this._holdRotation;
                }

                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, Time.deltaTime * this._interpolationSpeed);
            }
        }

        private bool TryFindNearestChild(FlexalonDragTarget dragTarget, out Transform nearestChild, out float distanceSquared)
        {
            var moveDirection = (this._target - this._lastTarget).normalized;
            nearestChild = null;
            distanceSquared = float.MaxValue;
            foreach (Transform child in dragTarget.transform)
            {
                var childPos = dragTarget.transform.localToWorldMatrix.MultiplyPoint(child.GetComponent<FlexalonResult>().TargetPosition);
                var toChild = (childPos - this._lastTarget).normalized;
                if (child == this._placeholder.transform || Vector3.Dot(toChild, moveDirection) > 0)
                {
                    var distSq = Vector3.SqrMagnitude(childPos - this._target);
                    if (distSq < distanceSquared)
                    {
                        distanceSquared = distSq;
                        nearestChild = child;
                    }
                }
            }

            return nearestChild != null;
        }

        // Find a drag target to insert into by checking if it contains the target point.
        private bool TryFindNearestDragTarget(FlexalonDragTarget currentDragTarget, out FlexalonDragTarget dragTarget, out Transform nearestChild)
        {
            if (!this.CanLeave(currentDragTarget))
            {
                dragTarget = currentDragTarget;
                this.TryFindNearestChild(currentDragTarget, out nearestChild, out var distanceSquared);
                return true;
            }

            dragTarget = null;
            nearestChild = null;
            var minDistance = float.MaxValue;
            this.GetInsertPositionAndRadius(this._node, this._target, out var worldInsertPosition, out var worldInsertRadius);

            foreach (var candidate in FlexalonDragTarget.DragTargets)
            {
                if (this.CanAdd(currentDragTarget, candidate) && candidate.OverlapsSphere(worldInsertPosition, worldInsertRadius))
                {
                    if (this.TryFindNearestChild(candidate, out var candidateNearestChild, out var distanceSquared))
                    {
                        if (distanceSquared < minDistance)
                        {
                            minDistance = distanceSquared;
                            dragTarget = candidate;
                            nearestChild = candidateNearestChild;
                        }
                    }
                    else if (dragTarget == null)
                    {
                        dragTarget = candidate;
                        break;
                    }
                }

            }

            return dragTarget != null;
        }

        // Moves the placeholder into the drag target at a particular index.
        private bool MovePlaceholder(Transform newParent, int siblingIndex = 0)
        {
            if (newParent != this._placeholder.transform.parent ||
                (newParent != null && siblingIndex != this._placeholder.transform.GetSiblingIndex()))
            {
                this._placeholder.SetActive(!!newParent);
                this._placeholder.transform.SetParent(newParent);
                if (newParent)
                {
                    this._placeholder.transform.SetSiblingIndex(siblingIndex);
                    this._lastValidLocalSpace = newParent;
                }

                this._localSpace = newParent;
                return true;
            }

            return false;
        }

        // Finds an appropriate place to add the placeholder into the drag target.
        private bool AddToLayout(FlexalonDragTarget currentDragTarget, FlexalonDragTarget newDragTarget, Transform nearestChild)
        {
            var insertIndex = nearestChild ? nearestChild.GetSiblingIndex() : 0;

            // Special case -- if adding a new item at the end, the user usually wants to place
            // it after the last element.
            if (currentDragTarget != newDragTarget && insertIndex == newDragTarget.transform.childCount - 1)
            {
                insertIndex++;
            }

            return this.MovePlaceholder(newDragTarget.transform, insertIndex);
        }

        private bool CanLeave(FlexalonDragTarget dragTarget)
        {
            return dragTarget == null ||
                (dragTarget.CanRemoveObjects && dragTarget.transform.childCount > dragTarget.MinObjects);
        }

        private bool CanAdd(FlexalonDragTarget currentDragTarget, FlexalonDragTarget dragTarget)
        {
            if (currentDragTarget == dragTarget)
            {
                return true;
            }

            return dragTarget != null &&
                dragTarget.gameObject != this.gameObject &&
                dragTarget.CanAddObjects  &&
                (dragTarget.MaxObjects == 0 || dragTarget.transform.childCount < dragTarget.MaxObjects) &&
                (this._layerMask.value & (1 << dragTarget.gameObject.layer)) != 0;
        }

        private void GetInsertPositionAndRadius(FlexalonNode node, Vector3 target, out Vector3 position, out float radius)
        {
            var worldBoxScale = node.GetWorldBoxScale(true);
            var scale = this.transform.lossyScale;
            radius = this._insertRadius * Mathf.Max(scale.x, scale.y, scale.z);
            position = target;
        }

        private void OnDrawGizmosSelected()
        {
            var node = Core.Flexalon.GetNode(this.gameObject);
            if (node != null && this._draggable)
            {
                Gizmos.color = Color.green;
                var target = this._state == InteractableState.Dragging ? this._target : this.transform.position;
                this.GetInsertPositionAndRadius(node, target, out var insertPosition, out var insertRadius);
                Gizmos.DrawWireSphere(insertPosition, insertRadius);
            }
        }
    }
}