using System.Collections.Generic;
using Flexalon.Runtime.Core;
using UnityEngine;

namespace Flexalon.Runtime.Interaction
{
    /// <summary> A drag target allows a layout to accept  dragged FlexalonInteractable objects. </summary>
    [AddComponentMenu("Flexalon/Flexalon Drag Target"), HelpURL("https://www.flexalon.com/docs/dragging"), DisallowMultipleComponent]
    public class FlexalonDragTarget : MonoBehaviour
    {
        [SerializeField]
        private bool _canRemoveObjects = true;
        /// <summary> Whether objects can be removed from the layout by dragging them from this target. </summary>
        public bool CanRemoveObjects {
            get => this._canRemoveObjects;
            set => this._canRemoveObjects = value;
        }

        [SerializeField]
        private bool _canAddObjects = true;
        /// <summary> Whether objects can be added to the layout by dragging them to this target. </summary>
        public bool CanAddObjects {
            get => this._canAddObjects;
            set => this._canAddObjects = value;
        }

        [SerializeField]
        private int _minObjects;
        /// <summary> The minimum number of objects that must remain in this layout. </summary>
        public int MinObjects {
            get => this._minObjects;
            set => this._minObjects = value;
        }

        [SerializeField]
        private int _maxObjects;
        /// <summary> The maximum number of objects that can be added to the layout. </summary>
        public int MaxObjects {
            get => this._maxObjects;
            set => this._maxObjects = value;
        }

        [SerializeField]
        private Vector3 _margin;
        /// <summary> Extra margin around the layout size to use for hit testing. </summary>
        public Vector3 Margin {
            get => this._margin;
            set => this._margin = value;
        }

        private FlexalonNode _node;

        private static HashSet<FlexalonDragTarget> _dragTargets = new HashSet<FlexalonDragTarget>();
        public static IReadOnlyCollection<FlexalonDragTarget> DragTargets => FlexalonDragTarget._dragTargets;

        void OnEnable()
        {
            this._node = Core.Flexalon.GetOrCreateNode(this.gameObject);
            FlexalonDragTarget._dragTargets.Add(this);
        }

        void OnDisable()
        {
            this._node = null;
            FlexalonDragTarget._dragTargets.Remove(this);
        }

        internal bool OverlapsSphere(Vector3 position, float radius)
        {
            var center = this._node.Result.AdapterBounds.center;
            var extents = (this._node.Result.AdapterBounds.size + this._margin * 2) / 2;
            var min = center - extents;
            var max = center + extents;

            // Transform the sphere center into the OBB's local coordinate system
            Vector3 localSphereCenter = this.transform.InverseTransformPoint(position);

            // Calculate the closest point on the OBB to the sphere center
            Vector3 closestPointOnOBB = Vector3.Min(Vector3.Max(localSphereCenter, min), max);

            // Calculate the distance between the closest point and the sphere center
            float distanceSquared = (closestPointOnOBB - localSphereCenter).sqrMagnitude;

            // Check if the distance is less than or equal to the sphere's radius squared
            return distanceSquared <= radius * radius;
        }
    }
}