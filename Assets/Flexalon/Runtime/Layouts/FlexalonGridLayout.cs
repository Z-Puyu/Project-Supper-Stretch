using System;
using System.Collections.Generic;
using Flexalon.Runtime.Core;
using UnityEngine;
using Math = Flexalon.Runtime.Core.Math;

namespace Flexalon.Runtime.Layouts
{
    /// <summary>
    /// Use a grid layout to position children at fixed intervals.
    /// Objects are placed in cells in column-row-layer order.
    /// </summary>
    [AddComponentMenu("Flexalon/Flexalon Grid Layout"), HelpURL("https://www.flexalon.com/docs/gridLayout")]
    public class FlexalonGridLayout : LayoutBase
    {
        /// <summary> The type of cell to use on the column-row axes. </summary>
        public enum CellTypes
        {
            /// <summary> A rectangular cell. </summary>
            Rectangle,

            /// <summary> A hexagonal cell. </summary>
            Hexagonal
        }

        [SerializeField]
        private CellTypes _cellType = CellTypes.Rectangle;
        /// <summary> The type of cell to use on the column-row axes. </summary>
        public CellTypes CellType
        {
            get { return this._cellType; }
            set { this._cellType = value; this._node.MarkDirty(); }
        }

        [SerializeField, Min(1)]
        private uint _columns = 3;
        /// <summary> The number of columns in the grid. </summary>
        public uint Columns
        {
            get { return this._columns; }
            set { this._columns = System.Math.Max(value, 1); this._node.MarkDirty(); }
        }

        [SerializeField, Min(1)]
        private uint _rows = 3;
        /// <summary> The number of rows in the grid. </summary>
        public uint Rows
        {
            get { return this._rows; }
            set { this._rows = System.Math.Max(value, 1); this._node.MarkDirty(); }
        }

        [SerializeField, Min(1)]
        private uint _layers = 1;
        /// <summary> The number of layers in the grid. </summary>
        public uint Layers
        {
            get { return this._layers; }
            set { this._layers = System.Math.Max(value, 1); this._node.MarkDirty(); }
        }

        [SerializeField]
        private Direction _columnDirection = Direction.PositiveX;
        /// <summary> The direction of the column axis. </summary>
        public Direction ColumnDirection
        {
            get { return this._columnDirection; }
            set { this._columnDirection = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Direction _rowDirection = Direction.NegativeY;
        /// <summary> The direction of the row axis. </summary>
        public Direction RowDirection
        {
            get { return this._rowDirection; }
            set { this._rowDirection = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Direction _layerDirection = Direction.PositiveZ;
        /// <summary> The direction of the layer axis. </summary>
        public Direction LayerDirection
        {
            get { return this._layerDirection; }
            set { this._layerDirection = value; this._node.MarkDirty(); }
        }

        /// <summary> How to determine the size of the cell. </summary>
        public enum CellSizeTypes
        {
            /// <summary> The object size is divided by the number of columns. </summary>
            Fill,

            /// <summary> The cell size is fixed. </summary>
            Fixed,
        }

        [SerializeField]
        private CellSizeTypes _columnSizeType = CellSizeTypes.Fill;
        /// <summary> How to determine the size of the columns, </summary>
        public CellSizeTypes ColumnSizeType
        {
            get { return this._columnSizeType; }
            set { this._columnSizeType = value; this._node.MarkDirty(); }
        }

        [SerializeField, Min(0)]
        private float _columnSize = 1.0f;
        /// <summary> The fixed size of the columns. </summary>
        public float ColumnSize
        {
            get { return this._columnSize; }
            set
            {
                this._columnSize = Mathf.Max(0, value);
                this._columnSizeType = CellSizeTypes.Fixed;
                this._node.MarkDirty();
            }
        }

        [SerializeField]
        private CellSizeTypes _rowSizeType = CellSizeTypes.Fill;
        /// <summary> How to determine the size of the rows. </summary>
        public CellSizeTypes RowSizeType
        {
            get { return this._rowSizeType; }
            set { this._rowSizeType = value; this._node.MarkDirty(); }
        }

        [SerializeField, Min(0)]
        private float _rowSize = 1.0f;
        /// <summary> The fixed size of the rows. </summary>
        public float RowSize
        {
            get { return this._rowSize; }
            set
            {
                this._rowSize = Mathf.Max(0, value);
                this._rowSizeType = CellSizeTypes.Fixed;
                this._node.MarkDirty();
            }
        }

        [SerializeField]
        private CellSizeTypes _layerSizeType = CellSizeTypes.Fill;
        /// <summary> How to determine the size of the layers. </summary>
        public CellSizeTypes LayerSizeType
        {
            get { return this._layerSizeType; }
            set { this._layerSizeType = value; this._node.MarkDirty(); }
        }

        [SerializeField, Min(0)]
        private float _layerSize = 1.0f;
        /// <summary> The fixed size of the layers. </summary>
        public float LayerSizeSize
        {
            get { return this._layerSize; }
            set
            {
                this._layerSize = Mathf.Max(0, value);
                this._layerSizeType = CellSizeTypes.Fixed;
                this._node.MarkDirty();
            }
        }

        [SerializeField]
        private float _columnSpacing = 0;
        /// <summary> The spacing between columns. </summary>
        public float ColumnSpacing
        {
            get { return this._columnSpacing; }
            set { this._columnSpacing = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private float _rowSpacing = 0;
        /// <summary> The spacing between rows. </summary>
        public float RowSpacing
        {
            get { return this._rowSpacing; }
            set { this._rowSpacing = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private float _layerSpacing = 0;
        /// <summary> The spacing between layers. </summary>
        public float LayerSpacing
        {
            get { return this._layerSpacing; }
            set { this._layerSpacing = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _horizontalAlign = Align.Center;
        /// <summary> How to align each child in its cell horizontally. </summary>
        public Align HorizontalAlign
        {
            get { return this._horizontalAlign; }
            set { this._horizontalAlign = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalAlign = Align.Center;
        /// <summary> How to align each child in its cell vertically. </summary>
        public Align VerticalAlign
        {
            get { return this._verticalAlign; }
            set { this._verticalAlign = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _depthAlign = Align.Center;
        /// <summary> How to align each child in its cell in depth. </summary>
        public Align DepthAlign
        {
            get { return this._depthAlign; }
            set { this._depthAlign = value; this._node.MarkDirty(); }
        }

        [Serializable]
        private class TransformList
        {
            public List<Transform> Items;
        }

        [Serializable]
        private class CellDict : FlexalonDict<Vector3Int, TransformList> {}

        [SerializeField, HideInInspector]
        private CellDict _cellToChildren;

        private Dictionary<Transform, Vector3Int> _childToCell;

        /// <summary> Returns the first child in the cell. </summary>
        /// <param name="column"> The column of the cell. </param>
        /// <param name="row"> The row of the cell. </param>
        /// <param name="layer"> The layer of the cell. </param>
        /// <returns> The first child in the cell. </returns>
        public Transform GetChildAt(int column, int row, int layer = 0)
        {
            if (this._cellToChildren != null)
            {
                if (this._cellToChildren.TryGetValue(new Vector3Int(column, row, layer), out var children))
                {
                    return children.Items[0];
                }
            }

            return null;
        }

        /// <summary> Returns all children in the cell. </summary>
        /// <param name="column"> The column of the cell. </param>
        /// <param name="row"> The row of the cell. </param>
        /// <param name="layer"> The layer of the cell. </param>
        /// <returns> A list of children in the cell. </returns>
        public Transform[] GetChildrenAt(int column, int row, int layer = 0)
        {
            if (this._cellToChildren != null)
            {
                if (this._cellToChildren.TryGetValue(new Vector3Int(column, row, layer), out var children))
                {
                    return children.Items.ToArray();
                }
            }

            return new Transform[0];
        }

        private void SetCell(Vector3Int cell, Transform child)
        {
            if (!this._cellToChildren.TryGetValue(cell, out var list))
            {
                list = new TransformList { Items = new List<Transform>() };
                this._cellToChildren.Add(cell, list);
            }

            list.Items.Add(child);
            this._childToCell.Add(child, cell);
        }

        private void UpdateCells()
        {
            if (this._cellToChildren == null)
            {
                this._cellToChildren = new CellDict();
            }

            if (this._childToCell == null)
            {
                this._childToCell = new Dictionary<Transform, Vector3Int>();
            }

            this._cellToChildren.Clear();
            this._childToCell.Clear();

            Vector3Int nextCell = Vector3Int.zero;
            foreach (var child in this.Node.Children)
            {
                var childTransform = child.GameObject.transform;
                if (child.GameObject.TryGetComponent<FlexalonGridCell>(out var cellComponent))
                {
                    this.SetCell(cellComponent.Cell, childTransform);
                }
                else
                {
                    this.SetCell(nextCell, childTransform);
                    nextCell[0]++;
                    if (nextCell[0] >= this._columns)
                    {
                        nextCell[0] = 0;
                        nextCell[1]++;
                        if (nextCell[1] >= this._rows)
                        {
                            nextCell[1] = 0;
                            nextCell[2]++;
                        }
                    }
                }
            }
        }

        private Vector3 GetCellSize(Vector3Int axes, Vector3 layoutSize)
        {
            var cellSize = layoutSize;
            cellSize[axes[0]] = this.GetColumnSize(layoutSize[axes[0]]);
            cellSize[axes[1]] = this.GetRowSize(layoutSize[axes[1]]);
            cellSize[axes[2]] = this.GetLayerSize(layoutSize[axes[2]]);
            return cellSize;
        }

        private Vector3 GetGridSize(Vector3Int axes, Vector3 cellSize)
        {
            int columnAxis = axes[0];
            int rowAxis = axes[1];
            int layerAxis = axes[2];

            Vector3 gridSize = Vector3.zero;
            if (this._cellType == CellTypes.Hexagonal && this._rows > 1)
            {
                gridSize[rowAxis] = cellSize[rowAxis] + (0.75f * cellSize[rowAxis] + this._rowSpacing) * (this._rows - 1);
            }
            else
            {
                gridSize[rowAxis] = cellSize[rowAxis] * this._rows + (this._rowSpacing * (this._rows - 1));
            }

            gridSize[columnAxis] = cellSize[columnAxis] * this._columns + (this._columnSpacing * (this._columns - 1));
            if (this._cellType == CellTypes.Hexagonal && this._rows > 1)
            {
                gridSize[columnAxis] += cellSize[columnAxis] * 0.5f;
            }

            gridSize[layerAxis] = cellSize[layerAxis] * this._layers + (this._layerSpacing * (this._layers - 1));
            return gridSize;
        }

        private Vector3Int GetAxes()
        {
            var columnAxis = (int) Math.GetAxisFromDirection(this._columnDirection);
            var rowAxis = (int) Math.GetAxisFromDirection(this._rowDirection);
            var layerAxis = (int) Math.GetAxisFromDirection(this._layerDirection);

            var otherAxes = Math.GetOtherAxes(columnAxis);
            if (columnAxis == rowAxis)
            {
                rowAxis = (layerAxis == otherAxes.Item1) ? otherAxes.Item2 : otherAxes.Item1;
            }

            if (columnAxis == layerAxis)
            {
                layerAxis = (rowAxis == otherAxes.Item1) ? otherAxes.Item2 : otherAxes.Item1;
            }

            if (rowAxis == layerAxis)
            {
                layerAxis = Math.GetThirdAxis(columnAxis, rowAxis);
            }

            return new Vector3Int(columnAxis, rowAxis, layerAxis);
        }

        private CellSizeTypes[] GetCellSizeTypes()
        {
            return new CellSizeTypes[3] {
                this._columnSizeType,
                this._rowSizeType,
                this._layerSizeType
            };
        }

        /// <inheritdoc />
        public override Bounds Measure(FlexalonNode node, Vector3 size, Vector3 min, Vector3 max)
        {
            FlexalonLog.Log("GridMeasure | Size", node, size, min, max);

            var axes = this.GetAxes();
            var cellSize = this.GetCellSize(axes, size);
            var sizeTypes = this.GetCellSizeTypes();

            foreach (var child in node.Children)
            {
                var childSize = child.GetMeasureSize(size);
                for (int i = 0; i < 3; i++)
                {
                    if (node.GetSizeType(i) == SizeType.Layout && sizeTypes[i] == CellSizeTypes.Fill)
                    {
                        cellSize[i] = Mathf.Max(childSize[i], cellSize[i]);
                    }
                }
            }

            var minCellSize = this.GetCellSize(axes, min);
            var maxCellSize = this.GetCellSize(axes, max);
            cellSize = Math.Clamp(cellSize, minCellSize, maxCellSize);

            var gridSize = this.GetGridSize(axes, cellSize);
            for (int i = 0; i < 3; i++)
            {
                if (node.GetSizeType(i) == SizeType.Layout)
                {
                    size[i] = gridSize[i];
                }
            }

            this.SetChildrenFillShrinkSize(node, cellSize, size);
            return new Bounds(Vector3.zero, size);
        }

        private float GetRowSize(float availableRowSize)
        {
            if (this._rowSizeType == CellSizeTypes.Fixed)
            {
                return this._rowSize;
            }

            if (this._cellType == CellTypes.Rectangle)
            {
                return (availableRowSize - this._rowSpacing * (this._rows - 1)) / this._rows;
            }
            else
            {
                return (availableRowSize - this._rowSpacing * (this._rows - 1)) / (1 + (this._rows - 1) * 0.75f);
            }
        }

        private float GetColumnSize(float availableColumnSize)
        {
            if (this._columnSizeType == CellSizeTypes.Fixed)
            {
                return this._columnSize;
            }

            if (this._cellType == CellTypes.Rectangle)
            {
                return (availableColumnSize - this._columnSpacing * (this._columns - 1)) / this._columns;
            }
            else
            {
                var sz = (availableColumnSize - this._columnSpacing * (this._columns - 1)) / this._columns;
                if (this._rows > 1)
                {
                    sz *= this._columns / (this._columns + 0.5f);
                }

                return sz;
            }
        }

        private float GetLayerSize(float availableColumnSize)
        {
            if (this._layerSizeType == CellSizeTypes.Fixed)
            {
                return this._layerSize;
            }

            return (availableColumnSize - this._layerSpacing * (this._layers - 1)) / this._layers;
        }

        private Vector3 GetPosition(Vector3Int cell, Vector3Int axes, Vector3 cellSize, Vector3 gridSize)
        {
            var columnAxis = axes[0];
            var rowAxis = axes[1];
            var layerAxis = axes[2];

            var columnSize = cellSize[axes[0]];
            var rowSize = cellSize[axes[1]];
            var layerSize = cellSize[axes[2]];

            var position = -gridSize / 2;

            if (this._cellType == CellTypes.Rectangle)
            {
                position[rowAxis] += rowSize * cell[1] + this._rowSpacing * cell[1] + rowSize / 2;
                position[columnAxis] += columnSize * cell[0] + this._columnSpacing * cell[0] + columnSize / 2;
            }
            else
            {
                bool rowEven = (cell[1] % 2) == 0;
                position[axes[1]] += rowSize * 0.75f * cell[1] + this._rowSpacing * cell[1] + rowSize / 2;
                position[columnAxis] += columnSize * cell[0] + columnSize / 2 + this._columnSpacing * cell[0] + (rowEven ? 0 : columnSize / 2);
            }

            position[layerAxis] += layerSize * cell[2] + this._layerSpacing * cell[2] + layerSize / 2;

            position[rowAxis] *= Math.GetPositiveFromDirection(this._rowDirection);
            position[columnAxis] *= Math.GetPositiveFromDirection(this._columnDirection);
            position[layerAxis] *= Math.GetPositiveFromDirection(this._layerDirection);
            return position;
        }

        private void PositionChild(FlexalonNode child, Vector3Int cell, Vector3Int axes, Vector3 cellSize, Vector3 gridSize)
        {
            Vector3 position;
            position = this.GetPosition(cell, axes, cellSize, gridSize);
            var aligned = Math.Align(child.GetArrangeSize(), cellSize, this._horizontalAlign, this._verticalAlign, this._depthAlign);
            child.SetPositionResult(position + aligned);
        }

        /// <inheritdoc />
        public override void Arrange(FlexalonNode node, Vector3 layoutSize)
        {
            FlexalonLog.Log("GridArrange | LayoutSize", node, layoutSize);

            var axes = this.GetAxes();
            var cellSize = this.GetCellSize(axes, layoutSize);
            var gridSize = this.GetGridSize(axes, cellSize);

            this.UpdateCells();

            foreach (var child in node.Children)
            {
                if (this._childToCell.TryGetValue(child.GameObject.transform, out var cell))
                {
                    this.PositionChild(child, cell, axes, cellSize, gridSize);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (this._node != null)
            {
                var axes = this.GetAxes();
                var sz = this._node.Result.AdapterBounds.size - this._node.Padding.Size;
                var cellSize = this.GetCellSize(axes, sz);
                var gridSize = this.GetGridSize(axes, cellSize);

                Gizmos.color = new Color(1, 1, 0, 0.5f);
                var scale = this._node.GetWorldBoxScale(true);
                Gizmos.matrix = Matrix4x4.TRS(this._node.GetWorldBoxPosition(scale, true), this.transform.rotation, scale);

                for (int r = 0; r < this._rows; r++)
                {
                    for (int c = 0; c < this._columns; c++)
                    {
                        for (int l = 0; l < this._layers; l++)
                        {
                            var position = this.GetPosition(new Vector3Int(c, r, l), axes, cellSize, gridSize);
                            if (this._cellType == CellTypes.Rectangle)
                            {
                                this.DrawRectangle(position, axes, cellSize);
                            }
                            else
                            {
                                this.DrawHexagon(position, axes, cellSize);
                            }
                        }
                    }
                }
            }
        }

        void DrawRectangle(Vector3 position, Vector3Int axes, Vector3 cellSize)
        {
            var columnAxis = axes[0];
            var rowAxis = axes[1];

            var columnSize = cellSize[axes[0]];
            var rowSize = cellSize[axes[1]];

            var p1 = new Vector3(); // top right
            p1[rowAxis] = rowSize / 2;
            p1[columnAxis] = columnSize / 2;

            var p2 = new Vector3(); // bottom right
            p2[rowAxis] = -rowSize / 2;
            p2[columnAxis] = columnSize / 2;

            var p3 = new Vector3(); // bottom left
            p3[rowAxis] = -rowSize / 2;
            p3[columnAxis] = -columnSize / 2;

            var p4 = new Vector3(); // top left
            p4[rowAxis] = rowSize / 2;
            p4[columnAxis] = -columnSize / 2;

            Gizmos.DrawLine(position + p1, position + p2);
            Gizmos.DrawLine(position + p2, position + p3);
            Gizmos.DrawLine(position + p3, position + p4);
            Gizmos.DrawLine(position + p4, position + p1);
        }

        void DrawHexagon(Vector3 position, Vector3Int axes, Vector3 cellSize)
        {
            var columnAxis = axes[0];
            var rowAxis = axes[1];

            var columnSize = cellSize[axes[0]];
            var rowSize = cellSize[axes[1]];

            var p1 = new Vector3(); // top
            p1[rowAxis] = rowSize / 2;

            var p2 = new Vector3(); // top right
            p2[rowAxis] = rowSize / 4;
            p2[columnAxis] = columnSize / 2;

            var p3 = new Vector3(); // bottom right
            p3[rowAxis] = -rowSize / 4;
            p3[columnAxis] = columnSize / 2;

            var p4 = new Vector3(); // bottom
            p4[rowAxis] = -rowSize / 2;

            var p5 = new Vector3(); // bottom left
            p5[rowAxis] = -rowSize / 4;
            p5[columnAxis] = -columnSize / 2;

            var p6 = new Vector3(); // top left
            p6[rowAxis] = rowSize / 4;
            p6[columnAxis] = -columnSize / 2;

            Gizmos.DrawLine(position + p1, position + p2);
            Gizmos.DrawLine(position + p2, position + p3);
            Gizmos.DrawLine(position + p3, position + p4);
            Gizmos.DrawLine(position + p4, position + p5);
            Gizmos.DrawLine(position + p5, position + p6);
            Gizmos.DrawLine(position + p6, position + p1);
        }
    }
}