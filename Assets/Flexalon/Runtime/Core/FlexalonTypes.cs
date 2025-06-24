using UnityEngine;

namespace Flexalon.Runtime.Core
{
    /// <summary> Represents an axis and direction. </summary>
    public enum Direction
    {
        PositiveX = 0,
        NegativeX = 1,
        PositiveY = 2,
        NegativeY = 3,
        PositiveZ = 4,
        NegativeZ = 5
    };

    /// <summary> Represents an axis. </summary>
    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2
    };

    /// <summary> Represents a direction to align. </summary>
    public enum Align
    {
        Start = 0,
        Center = 1,
        End = 2
    };

    /// <summary> Represents a plane along two axes. </summary>
    public enum Plane
    {
        XY = 0,
        XZ = 1,
        ZY = 2
    }

    /// <summary> Determines how a FlexalonObject should be sized. </summary>
    public enum SizeType
    {
        /// <summary> Specify a fixed size value. </summary>
        Fixed = 0,

        /// <summary> Specify a factor of the space allocated by the parent layout.
        /// For example, 0.5 will fill half of the space. </summary>
        Fill = 1,

        /// <summary> The size is determined by the Adapter and attached Unity
        /// components such as MeshRenderer, SpriteRenderer, TMP_Text, RectTransform, and Colliders.
        /// An empty GameObject gets a size of 1. </summary>
        Component = 2,

        /// <summary> The size determined by the layout's algorithm. </summary>
        Layout = 3
    };

    /// <summary> Determines how a FlexalonObject min or max should be determined. </summary>
    public enum MinMaxSizeType
    {
        /// <summary> For min, the object cannot shrink. For max, this is infinity. </summary>
        None = 0,

        /// <summary> Specify a fixed min or max size value. </summary>
        Fixed = 1,

        /// <summary> Specify a factor of the space allocated by the parent layout.
        /// For example, 0.5 will fill half of the space. </summary>
        Fill = 2
    };

    /// <summary> Six floats representing right, left, top, bottom, back, front.</summary>
    [System.Serializable]
    public struct Directions
    {
        private static Directions _zero = new Directions(new float[] { 0, 0, 0, 0, 0, 0 });
        public static Directions zero => Directions._zero;

        private float[] _values;

        public float Right
        {
            get => this._values[0];
            set => this._values[0] = value;
        }

        public float Left
        {
            get => this._values[1];
            set => this._values[1] = value;
        }

        public float Top
        {
            get => this._values[2];
            set => this._values[2] = value;
        }

        public float Bottom
        {
            get => this._values[3];
            set => this._values[3] = value;
        }

        public float Back
        {
            get => this._values[4];
            set => this._values[4] = value;
        }

        public float Front
        {
            get => this._values[5];
            set => this._values[5] = value;
        }

        public Directions(params float[] values)
        {
            this._values = values;
        }

        public float this[int key]
        {
            get => this._values[key];
        }

        public float this[Direction key]
        {
            get => this._values[(int)key];
        }

        public Vector3 Size => new Vector3(
            this._values[0] + this._values[1], this._values[2] + this._values[3], this._values[4] + this._values[5]);

        public Vector3 Center => new Vector3(
            (this._values[0] - this._values[1]) * 0.5f, (this._values[2] - this._values[3]) * 0.5f, (this._values[4] - this._values[5]) * 0.5f);

        public override bool Equals(object obj)
        {
            if (obj is Directions other)
            {
                return this._values[0] == other._values[0] && this._values[1] == other._values[1] && this._values[2] == other._values[2] &&
                    this._values[3] == other._values[3] && this._values[4] == other._values[4] && this._values[5] == other._values[5];
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Directions a, Directions b)
        {
            return Mathf.Approximately(a._values[0], b._values[0]) &&
                Mathf.Approximately(a._values[1], b._values[1]) &&
                Mathf.Approximately(a._values[2], b._values[2]) &&
                Mathf.Approximately(a._values[3], b._values[3]) &&
                Mathf.Approximately(a._values[4], b._values[4]) &&
                Mathf.Approximately(a._values[5], b._values[5]);
        }

        public static bool operator !=(Directions a, Directions b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"({this._values[0]}, {this._values[1]}, {this._values[2]}, {this._values[3]}, {this._values[4]}, {this._values[5]})";
        }
    }
}