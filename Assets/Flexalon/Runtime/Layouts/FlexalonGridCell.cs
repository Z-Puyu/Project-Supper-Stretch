using Flexalon.Runtime.Core;
using UnityEngine;

namespace Flexalon.Runtime.Layouts
{
    /// <summary> Specifies which cell a gameObject should occupy in a grid layout. </summary>
    [AddComponentMenu("Flexalon/Flexalon Grid Cell"), HelpURL("https://www.flexalon.com/docs/gridLayout")]
    public class FlexalonGridCell : FlexalonComponent
    {
        [SerializeField, Min(0)]
        private int _column;
        /// <summary> The column of the cell. </summary>
        public int Column
        {
            get => this._column;
            set
            {
                this._column = Mathf.Max(0, value);
                this.MarkDirty();
            }
        }

        [SerializeField, Min(0)]
        private int _row;
        /// <summary> The row of the cell. </summary>
        public int Row
        {
            get => this._row;
            set
            {
                this._row = Mathf.Max(0, value);
                this.MarkDirty();
            }
        }

        [SerializeField, Min(0)]
        private int _layer;
        /// <summary> The layer of the cell. </summary>
        public int Layer
        {
            get => this._layer;
            set
            {
                this._layer = Mathf.Max(0, value);
                this.MarkDirty();
            }
        }

        /// <summary> The cell to occupy. </summary>
        public Vector3Int Cell
        {
            get => new Vector3Int(this._column, this._row, this._layer);
            set
            {
                this._column = Mathf.Max(0, value.x);
                this._row = Mathf.Max(0, value.y);
                this._layer = Mathf.Max(0, value.z);
                this.MarkDirty();
            }
        }
    }
}