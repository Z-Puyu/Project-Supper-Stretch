using System.Collections.Generic;
using Flexalon.Runtime.Core;
using UnityEngine;

namespace Flexalon.Runtime.Layouts
{
    /// <summary>
    /// Use a flexible layout to position children linearly along the x, y, or z axis.
    /// The sizes of the children are considered so that they are evenly spaced.
    /// </summary>
    [AddComponentMenu("Flexalon/Flexalon Flexible Layout"), HelpURL("https://www.flexalon.com/docs/flexibleLayout")]
    public class FlexalonFlexibleLayout : LayoutBase
    {
        /// <summary> Determines how the space between children is distributed. </summary>
        public enum GapOptions
        {
            /// <summary> The Gap/WrapGap property determines the space between children. </summary>
            Fixed,

            /// <summary> Space is added between children to fill the available space. </summary>
            SpaceBetween
        }

        [SerializeField]
        private Direction _direction = Direction.PositiveX;
        /// <summary> The direction in which objects are placed, one after the other. </summary>
        public Direction Direction
        {
            get { return this._direction; }
            set { this._direction = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private bool _wrap;
        /// <summary> If set, then the flexible layout will attempt to position children in a line
        /// along the Direction axis until it runs out of space. Then it will start the next line by
        /// following the wrap direction. Wrapping will only occur if the size of the Direction axis is
        /// set to any value other than "Layout". </summary>
        public bool Wrap
        {
            get { return this._wrap; }
            set { this._wrap = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Direction _wrapDirection = Direction.NegativeY;
        /// <summary> The direction to start a new line when wrapping. </summary>
        public Direction WrapDirection
        {
            get { return this._wrapDirection; }
            set { this._wrapDirection = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _horizontalAlign = Align.Center;
        /// <summary> Determines how the entire layout horizontally aligns to the parent's box. </summary>
        public Align HorizontalAlign
        {
            get { return this._horizontalAlign; }
            set { this._horizontalAlign = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalAlign = Align.Center;
        /// /// <summary> Determines how the entire layout vertically aligns to the parent's box. </summary>
        public Align VerticalAlign
        {
            get { return this._verticalAlign; }
            set { this._verticalAlign = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _depthAlign = Align.Center;
        /// <summary> Determines how the entire layout aligns to the parent's box in depth. </summary>
        public Align DepthAlign
        {
            get { return this._depthAlign; }
            set { this._depthAlign = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _horizontalInnerAlign = Align.Center;
        /// <summary> The inner align property along the Direction axis will change how wrapped lines align
        /// with each other. The inner align property along the other two axes will change how each object lines
        /// up with all other objects. </summary>
        public Align HorizontalInnerAlign
        {
            get { return this._horizontalInnerAlign; }
            set { this._horizontalInnerAlign = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalInnerAlign = Align.Center;
        /// <summary> The inner align property along the Direction axis will change how wrapped lines align
        /// with each other. The inner align property along the other two axes will change how each object lines
        /// up with all other objects. </summary>
        public Align VerticalInnerAlign
        {
            get { return this._verticalInnerAlign; }
            set { this._verticalInnerAlign = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private Align _depthInnerAlign = Align.Center;
        /// <summary> The inner align property along the Direction axis will change how wrapped lines align
        /// with each other. The inner align property along the other two axes will change how each object lines
        /// up with all other objects. </summary>
        public Align DepthInnerAlign
        {
            get { return this._depthInnerAlign; }
            set { this._depthInnerAlign = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private GapOptions _gapType = GapOptions.Fixed;
        /// <summary> Determines how the space between children is distributed. </summary>
        public GapOptions GapType
        {
            get { return this._gapType; }
            set { this._gapType = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private float _gap;
        /// <summary> Adds a gap between objects on the Direction axis. </summary>
        public float Gap
        {
            get { return this._gap; }
            set
            {
                this._gap = value;
                this._gapType = GapOptions.Fixed;
                this._node.MarkDirty();
            }
        }

        [SerializeField]
        private GapOptions _wrapGapType = GapOptions.Fixed;
        /// <summary> Determines how the space between lines is distributed. </summary>
        public GapOptions WrapGapType
        {
            get { return this._wrapGapType; }
            set { this._wrapGapType = value; this._node.MarkDirty(); }
        }

        [SerializeField]
        private float _wrapGap;
        /// <summary> Adds a gap between objects on the Wrap Direction axis. </summary>
        public float WrapGap
        {
            get { return this._wrapGap; }
            set
            {
                this._wrapGap = value;
                this._wrapGapType = GapOptions.Fixed;
                this._node.MarkDirty();
            }
        }

        private class Line
        {
            public Vector3 Size = Vector3.zero;
            public Vector3 Position = Vector3.zero;
            public List<FlexalonNode> Children = new List<FlexalonNode>();
            public List<Vector3> ChildSizes = new List<Vector3>();
            public List<Vector3> ChildPositions = new List<Vector3>();
        }

        private List<Line> _lines = new List<Line>();
        private List<FlexItem> _flexItems = new List<FlexItem>();

        private void CreateLines(FlexalonNode node, int flexAxis, int wrapAxis, int thirdAxis, bool wrap, Vector3 size, float maxLineSize, bool measure)
        {
            this._lines.Clear();
            if (node.Children.Count == 0)
            {
                return;
            }

            // Divide children into lines considering: size, child sizes.
            var line = new Line();
            this._lines.Add(line);
            bool addGap = false;
            int i = 0;
            foreach (var child in node.Children)
            {
                var gap = (addGap && this._gapType == GapOptions.Fixed ? this._gap : 0);
                var childSize = measure ? child.GetMeasureSize(size) : child.GetArrangeSize();
                if (line.ChildSizes.Count > 0 && wrap &&
                    line.Size[flexAxis] + childSize[flexAxis] + gap > maxLineSize)
                {
                    line = new Line();
                    this._lines.Add(line);
                    addGap = false;
                    gap = 0;
                    i++;
                }

                FlexalonLog.Log("Flex | Add child to line", child, i);
                FlexalonLog.Log("Flex | Child Size", child, childSize);
                line.ChildSizes.Add(childSize);
                line.Size[flexAxis] += childSize[flexAxis] + gap;
                line.Size[wrapAxis] = Mathf.Max(line.Size[wrapAxis], childSize[wrapAxis]);
                line.Size[thirdAxis] = Mathf.Max(line.Size[thirdAxis], childSize[thirdAxis]);
                line.Children.Add(child);
                addGap = true;
            }
        }

        private Vector3 MeasureTotalLineSize(bool wrap, int flexAxis, int wrapAxis, int thirdAxis)
        {
            Vector3 layoutSize = Vector3.zero;
            foreach (var line in this._lines)
            {
                if (wrap)
                {
                    layoutSize[flexAxis] = Mathf.Max(layoutSize[flexAxis], line.Size[flexAxis]);
                    layoutSize[wrapAxis] += line.Size[wrapAxis];
                    layoutSize[thirdAxis] = Mathf.Max(layoutSize[thirdAxis], line.Size[thirdAxis]);
                }
                else
                {
                    for (int axis = 0; axis < 3; axis++)
                    {
                        layoutSize[axis] = Mathf.Max(layoutSize[axis], line.Size[axis]);
                    }
                }
            }

            if (wrap && this._wrapGapType == GapOptions.Fixed)
            {
                layoutSize[wrapAxis] += this._wrapGap * (this._lines.Count - 1);
            }

            return layoutSize;
        }

        private Vector3 MeasureLayoutSize(FlexalonNode node, bool wrap, int flexAxis, int wrapAxis, int thirdAxis, Vector3 size, Vector3 min, Vector3 max)
        {
            var layoutSize = this.MeasureTotalLineSize(wrap, flexAxis, wrapAxis, thirdAxis);

            for (int axis = 0; axis < 3; axis++)
            {
                if (node.GetSizeType((Axis)axis) == SizeType.Layout)
                {
                    layoutSize[axis] = Mathf.Clamp(layoutSize[axis], min[axis], max[axis]);
                }
                else
                {
                    layoutSize[axis] = size[axis];
                }
            }

            return layoutSize;
        }

        private void SetChildSize(Line line, int index, int axis, float size, float layoutSize)
        {
            var childSize = line.ChildSizes[index];
            line.Children[index].SetShrinkFillSize(axis, size, layoutSize, true);
            childSize[axis] = size;
            line.ChildSizes[index] = childSize;
        }

        private void FillFlexAxis(float size, int flexAxis)
        {
            var gap = this._gapType == GapOptions.Fixed ? this._gap : 0;

            foreach (var line in this._lines)
            {
                this._flexItems.Clear();
                for (int i = 0; i < line.Children.Count; i++)
                {
                    this._flexItems.Add(Flex.CreateFlexItem(
                        line.Children[i], flexAxis, line.ChildSizes[i][flexAxis], line.Size[flexAxis], size));
                }

                Flex.GrowOrShrink(this._flexItems, line.Size[flexAxis], size, gap);

                for (int i = 0; i < line.Children.Count; i++)
                {
                    this.SetChildSize(line, i, flexAxis, this._flexItems[i].FinalSize, size);
                }
            }
        }

        private void FillWrapAxis(float size, int wrapAxis)
        {
            this._flexItems.Clear();
            float remainingSpace = size;
            var gap = this._wrapGapType == GapOptions.Fixed ? this._wrapGap : 0;

            foreach (var line in this._lines)
            {
                var item = new FlexItem();
                item.StartSize = line.Size[wrapAxis];
                item.MaxSize = line.Children[0].GetMaxSize(wrapAxis, size);
                item.ShrinkFactor = line.Size[wrapAxis] / size;
                item.FinalSize = line.Size[wrapAxis];

                for (int i = 0; i < line.Children.Count; i++)
                {
                    var child = line.Children[i];
                    if (child.CanShrink(wrapAxis))
                    {
                        item.MinSize = Mathf.Max(child.GetMinSize(wrapAxis, size), item.MinSize);
                    }
                    else
                    {
                        item.MinSize = Mathf.Max(line.ChildSizes[i][wrapAxis], item.MinSize);
                    }

                    item.MaxSize = Mathf.Max(child.GetMaxSize(wrapAxis, size), item.MaxSize);

                    if (child.GetSizeType(wrapAxis) == SizeType.Fill)
                    {
                        item.GrowFactor = Mathf.Max(child.SizeOfParent[wrapAxis], item.GrowFactor);
                    }
                }

                remainingSpace -= line.Size[wrapAxis];
                this._flexItems.Add(item);
            }

            if (Mathf.Abs(remainingSpace) > 1e-6)
            {
                Flex.GrowOrShrink(this._flexItems, size - remainingSpace, size, gap);
            }

            for (int l = 0; l < this._lines.Count; l++)
            {
                var line = this._lines[l];
                var item = this._flexItems[l];

                for (int i = 0; i < line.Children.Count; i++)
                {
                    var child = line.Children[i];
                    if (child.GetSizeType(wrapAxis) == SizeType.Fill)
                    {
                        var newSize = child.SizeOfParent[wrapAxis] * size;
                        var minSize = child.GetMinSize(wrapAxis, size);
                        var maxSize = child.GetMaxSize(wrapAxis, size);
                        maxSize = Mathf.Min(maxSize, item.FinalSize);
                        newSize = Mathf.Clamp(newSize, minSize, maxSize);
                        this.SetChildSize(line, i, wrapAxis, newSize, size);
                    }
                    else
                    {
                        this.SetChildSize(line, i, wrapAxis, item.FinalSize, size);
                    }
                }
            }
        }

        private void FillThirdAxis(float size, int thirdAxis)
        {
            foreach (var child in this._node.Children)
            {
                child.SetShrinkFillSize(thirdAxis, size, size);
            }
        }

        private void UpdateFillSizes(Vector3 size, int flexAxis, int wrapAxis, int thirdAxis)
        {
            this.FillFlexAxis(size[flexAxis], flexAxis);
            this.FillWrapAxis(size[wrapAxis], wrapAxis);
            this.FillThirdAxis(size[thirdAxis], thirdAxis);
        }

        /// <inheritdoc />
        public override Bounds Measure(FlexalonNode node, Vector3 size, Vector3 min, Vector3 max)
        {
            FlexalonLog.Log("FlexMeasure", node, size, min, max);

            // Gather useful data
            var flexAxis = (int) Math.GetAxisFromDirection(this._direction);
            var otherAxes = Math.GetOtherAxes(flexAxis);
            bool childrenSizeFlexAxis = node.GetSizeType(flexAxis) == SizeType.Layout;
            var wrapAxis = (int) Math.GetAxisFromDirection(this._wrapDirection);
            if (wrapAxis == flexAxis)
            {
                wrapAxis = otherAxes.Item1;
            }

            var thirdAxis = (wrapAxis == otherAxes.Item1 ? otherAxes.Item2 : otherAxes.Item1);
            bool wrap = (flexAxis != wrapAxis) && this._wrap;
            var maxLineSize = childrenSizeFlexAxis ? max[flexAxis] : size[flexAxis];

            FlexalonLog.Log("FlexMeasure | Flex Axis", node,  flexAxis);
            FlexalonLog.Log("FlexMeasure | Wrap Axis", node,  wrapAxis);
            FlexalonLog.Log("FlexMeasure | Third Axis", node,  thirdAxis);
            FlexalonLog.Log("FlexMeasure | Wrap", node, wrap);

            this.CreateLines(node, flexAxis, wrapAxis, thirdAxis, wrap, size, maxLineSize, true);
            for (int i = 0; i < this._lines.Count; i++)
            {
                FlexalonLog.Log("FlexMeasure | Line size " + i + " " + this._lines[i].Size);
            }

            Vector3 layoutSize = this.MeasureLayoutSize(node, wrap, flexAxis, wrapAxis, thirdAxis, size, min, max);
            FlexalonLog.Log("FlexMeasure | Total Layout Size", node, layoutSize);

            this.UpdateFillSizes(layoutSize, flexAxis, wrapAxis, thirdAxis);

            return new Bounds(Vector3.zero, layoutSize);
        }

        /// <inheritdoc />
        public override void Arrange(FlexalonNode node, Vector3 layoutSize)
        {
            FlexalonLog.Log("FlexArrange | LayoutSize", node, layoutSize);

            // Gather useful data
            var flexAxis = (int) Math.GetAxisFromDirection(this._direction);
            bool childrenSizeFlexAxis = node.GetSizeType(flexAxis) == SizeType.Layout;
            var otherAxes = Math.GetOtherAxes(flexAxis);
            var wrapAxis = (int) Math.GetAxisFromDirection(this._wrapDirection);
            if (wrapAxis == flexAxis)
            {
                wrapAxis = otherAxes.Item1;
            }

            var thirdAxis = (wrapAxis == otherAxes.Item1 ? otherAxes.Item2 : otherAxes.Item1);
            bool wrap = (flexAxis != wrapAxis) && this._wrap;
            var flexDirection = Math.GetPositiveFromDirection(this._direction);
            var wrapDirection = Math.GetPositiveFromDirection(this._wrapDirection);
            var align = new Align[] { this._horizontalAlign, this._verticalAlign, this._depthAlign };
            var innerAlign = new Align[] { this._horizontalInnerAlign, this._verticalInnerAlign, this._depthInnerAlign };

            FlexalonLog.Log("FlexArrange | Flex Direction", node, this._direction);
            FlexalonLog.Log("FlexArrange | Wrap Direction", node, this._wrapDirection);
            FlexalonLog.Log("FlexArrange | Third Axis", node, thirdAxis);
            FlexalonLog.Log("FlexArrange | Wrap", node, wrap);

            this.CreateLines(node, flexAxis, wrapAxis, thirdAxis, wrap, layoutSize, layoutSize[flexAxis] + 1e-4f, false);

            // Position children within _lines. Consider: line size, child size, flexInnerAlign
            {
                foreach (var line in this._lines)
                {
                    float lineGap = 0;
                    if (line.Children.Count > 1)
                    {
                        switch (this._gapType)
                        {
                            case GapOptions.Fixed:
                                lineGap = this._gap;
                                break;
                            case GapOptions.SpaceBetween:
                                lineGap = (layoutSize[flexAxis] - line.Size[flexAxis]) / (line.Children.Count - 1);
                                line.Size[flexAxis] = layoutSize[flexAxis];
                                break;
                        }
                    }

                    float nextChildPosition = flexDirection * -line.Size[flexAxis] / 2;
                    foreach (var childSize in line.ChildSizes)
                    {
                        Vector3 childPosition = Vector3.zero;
                        childPosition[flexAxis] = nextChildPosition + flexDirection * childSize[flexAxis] / 2;
                        childPosition[otherAxes.Item1] = Math.Align(
                            childSize, line.Size, otherAxes.Item1, innerAlign[otherAxes.Item1]);
                        childPosition[otherAxes.Item2] = Math.Align(
                            childSize, line.Size, otherAxes.Item2, innerAlign[otherAxes.Item2]);
                        line.ChildPositions.Add(childPosition);
                        nextChildPosition += flexDirection * (childSize[flexAxis] + lineGap);
                    }
                }
            }

            for (int i = 0; i < this._lines.Count; i++)
            {
                for (int j = 0; j < this._lines[i].ChildPositions.Count; j++)
                {
                    FlexalonLog.Log("FlexArrange | Child Size", this._lines[i].Children[j], this._lines[i].ChildSizes[j]);
                    FlexalonLog.Log("FlexArrange | Child Position", this._lines[i].Children[j], this._lines[i].ChildPositions[j]);
                }
            }

            Vector3 totalLineSize = this.MeasureTotalLineSize(wrap, flexAxis, wrapAxis, thirdAxis);
            FlexalonLog.Log("FlexArrange | Total Line Size", node, totalLineSize);

            // Position lines in total line size, consider: totalLineSize, innerAlign
            {
                if (wrap)
                {
                    float wrapGap = 0;
                    if (this._lines.Count > 1)
                    {
                        switch (this._wrapGapType)
                        {
                            case GapOptions.Fixed:
                                wrapGap = this._wrapGap;
                                break;
                            case GapOptions.SpaceBetween:
                                wrapGap = (layoutSize[wrapAxis] - totalLineSize[wrapAxis]) / (this._lines.Count - 1);
                                totalLineSize[wrapAxis] = layoutSize[wrapAxis];
                                break;
                        }
                    }

                    float nextLinePosition = wrapDirection * -totalLineSize[wrapAxis] / 2;
                    foreach (var line in this._lines)
                    {
                        line.Position[wrapAxis] = nextLinePosition + wrapDirection * line.Size[wrapAxis] / 2;
                        line.Position[flexAxis] = Math.Align(
                            line.Size, totalLineSize, flexAxis, innerAlign[flexAxis]);
                        line.Position[thirdAxis] = Math.Align(
                            line.Size, totalLineSize, thirdAxis, innerAlign[thirdAxis]);
                        nextLinePosition += wrapDirection * line.Size[wrapAxis] + wrapGap * wrapDirection;
                    }
                }
                else
                {
                    for (int axis = 0; axis < 3; axis++)
                    {
                        this._lines[0].Position[axis] = Math.Align(
                            this._lines[0].Size, totalLineSize, axis, innerAlign[axis]);
                    }
                }
            }

            for (int i = 0; i < this._lines.Count; i++)
            {
                FlexalonLog.Log("FlexArrange | Line position " + i + " " + this._lines[i].Position);
            }

            // Align the total line size within the size
            Vector3 alignOffset = Vector3.zero;
            for (int axis = 0; axis < 3; axis++)
            {
                alignOffset[axis] = Math.Align(totalLineSize, layoutSize, axis, align[axis]);
            }

            FlexalonLog.Log("FlexArrange | alignOffset", node, alignOffset);

            // Assign final child positions
            int childIndex = 0;
            foreach (var line in this._lines)
            {
                foreach (var childPosition in line.ChildPositions)
                {
                    var child = node.Children[childIndex];
                    var result = alignOffset + line.Position + childPosition;
                    child.SetPositionResult(result);
                    child.SetRotationResult(Quaternion.identity);
                    FlexalonLog.Log("FlexArrange | FinalChildPosition", child, result);
                    childIndex++;
                }
            }

            this._lines.Clear();
        }
    }
}