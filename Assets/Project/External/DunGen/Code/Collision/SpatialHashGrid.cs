using System;
using System.Collections.Generic;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code.Collision
{
	/// <summary>
	/// A generic spatial hash grid that divides space into uniform cells for fast spatial queries.
	/// The grid operates on a 2D plane perpendicular to the specified up axis.
	/// </summary>
	/// <typeparam name="T">The type of objects stored in the grid.</typeparam>
	public class SpatialHashGrid<T>
	{
		private readonly Dictionary<long, List<T>> cells;
		private readonly float cellSize;
		private readonly Func<T, Bounds> getBounds;
		private readonly AxisDirection upAxis;
		private readonly (int, int) primaryAxes; // Indices for the two axes that form the 2D plane


		/// <summary>
		/// Constructs a spatial hash grid.
		/// </summary>
		/// <param name="cellSize">Size of each grid cell</param>
		/// <param name="getBounds">Delegate to extract bounds from objects</param>
		/// <param name="upDirection">The up axis direction (determines the 2D plane)</param>
		public SpatialHashGrid(float cellSize, Func<T, Bounds> getBounds, AxisDirection upDirection = AxisDirection.PosY)
		{
			this.cells = new Dictionary<long, List<T>>();
			this.cellSize = cellSize;
			this.getBounds = getBounds;
			this.upAxis = upDirection;
			
			// Determine which axes to use for the 2D grid based on up direction
			this.primaryAxes = this.GetPrimaryAxes(upDirection);
		}

		private (int, int) GetPrimaryAxes(AxisDirection upDirection)
		{
			switch (upDirection)
			{
				case AxisDirection.PosY:
				case AxisDirection.NegY:
					return (0, 2); // Use X and Z axes
				case AxisDirection.PosX:
				case AxisDirection.NegX:
					return (1, 2); // Use Y and Z axes
				case AxisDirection.PosZ:
				case AxisDirection.NegZ:
					return (0, 1); // Use X and Y axes
				default:
					throw new ArgumentException("Invalid axis direction", nameof(upDirection));
			}
		}

		private Vector2 GetGridPosition(Vector3 worldPos)
		{
			// Extract the two coordinates for our 2D plane based on the up axis
			float x = worldPos[this.primaryAxes.Item1];
			float y = worldPos[this.primaryAxes.Item2];
			return new Vector2(x, y);
		}

		private long GetCellKey(int x, int y)
		{
			return ((long)x << 32) | (y & 0xffffffffL);
		}

		/// <summary>
		/// Inserts an object into the spatial hash grid.
		/// </summary>
		public void Insert(T obj)
		{
			Bounds bounds = this.getBounds(obj);
			Vector2 min = this.GetGridPosition(bounds.min);
			Vector2 max = this.GetGridPosition(bounds.max);

			int minX = Mathf.FloorToInt(min.x / this.cellSize);
			int minY = Mathf.FloorToInt(min.y / this.cellSize);
			int maxX = Mathf.FloorToInt(max.x / this.cellSize);
			int maxY = Mathf.FloorToInt(max.y / this.cellSize);

			for (int y = minY; y <= maxY; y++)
			{
				for (int x = minX; x <= maxX; x++)
				{
					long key = this.GetCellKey(x, y);

					if (!this.cells.TryGetValue(key, out List<T> cell))
					{
						cell = new List<T>();
						this.cells[key] = cell;
					}

					cell.Add(obj);
				}
			}
		}

		/// <summary>
		/// Removes an object from the spatial hash grid.
		/// </summary>
		public bool Remove(T obj)
		{
			bool removed = false;
			Bounds bounds = this.getBounds(obj);
			Vector2 min = this.GetGridPosition(bounds.min);
			Vector2 max = this.GetGridPosition(bounds.max);

			int minX = Mathf.FloorToInt(min.x / this.cellSize);
			int minY = Mathf.FloorToInt(min.y / this.cellSize);
			int maxX = Mathf.FloorToInt(max.x / this.cellSize);
			int maxY = Mathf.FloorToInt(max.y / this.cellSize);

			for (int y = minY; y <= maxY; y++)
			{
				for (int x = minX; x <= maxX; x++)
				{
					Int64 key = this.GetCellKey(x, y);
					if (this.cells.TryGetValue(key, out List<T> cell))
					{
						if (cell.Remove(obj))
						{
							removed = true;
							if (cell.Count == 0)
							{
								this.cells.Remove(key);
							}
						}
					}
				}
			}
			return removed;
		}

		/// <summary>
		/// Queries the spatial hash grid for objects that might intersect with the specified bounds.
		/// </summary>
		public void Query(Bounds queryBounds, ref List<T> results)
		{
			Vector3 queryBoundsMin = queryBounds.min;
			Vector3 queryBoundsMax = queryBounds.max;

			Vector2 min = this.GetGridPosition(queryBoundsMin);
			Vector2 max = this.GetGridPosition(queryBoundsMax);

			int minX = Mathf.FloorToInt(min.x / this.cellSize);
			int minY = Mathf.FloorToInt(min.y / this.cellSize);
			int maxX = Mathf.FloorToInt(max.x / this.cellSize);
			int maxY = Mathf.FloorToInt(max.y / this.cellSize);

			for (int y = minY; y <= maxY; y++)
			{
				for (int x = minX; x <= maxX; x++)
				{
					long key = this.GetCellKey(x, y);

					if (this.cells.TryGetValue(key, out List<T> cell))
					{
						foreach (T obj in cell)
						{
							var objBounds = this.getBounds(obj);
							Vector3 objBoundsMin = objBounds.min;
							Vector3 objBoundsMax = objBounds.max;

							// Manual intersection test to avoid the overhead of Bounds.Intersects
							bool intersects = objBoundsMin.x <= queryBoundsMax.x &&
								objBoundsMax.x >= queryBoundsMin.x &&
								objBoundsMin.y <= queryBoundsMax.y &&
								objBoundsMax.y >= queryBoundsMin.y &&
								objBoundsMin.z <= queryBoundsMax.z &&
								objBoundsMax.z >= queryBoundsMin.z;

							if (intersects)
							{
								if (!results.Contains(obj))
									results.Add(obj);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Clears all objects from the grid.
		/// </summary>
		public void Clear()
		{
			this.cells.Clear();
		}

		/// <summary>
		/// Draws debug visualization of the grid and contained objects.
		/// </summary>
		/// <param name="duration">How long the debug lines should remain visible</param>
		public void DrawDebug(float duration = 0.0f)
		{
			// Get unique cell coordinates to draw grid
			var cellCoords = new HashSet<(int x, int y)>();
			foreach (var key in this.cells.Keys)
			{
				int x = (int)(key >> 32);
				int y = (int)(key & 0xffffffffL);
				cellCoords.Add((x, y));
			}

			// Draw grid cells
			foreach (var coord in cellCoords)
			{
				Vector3 min = Vector3.zero;
				Vector3 max = Vector3.zero;

				// Set the coordinates based on primary axes
				min[this.primaryAxes.Item1] = coord.x * this.cellSize;
				min[this.primaryAxes.Item2] = coord.y * this.cellSize;
				max[this.primaryAxes.Item1] = (coord.x + 1) * this.cellSize;
				max[this.primaryAxes.Item2] = (coord.y + 1) * this.cellSize;

				Vector3 p1 = min;
				Vector3 p2 = min;
				p2[this.primaryAxes.Item1] = max[this.primaryAxes.Item1];

				Vector3 p3 = max;
				Vector3 p4 = max;
				p4[this.primaryAxes.Item1] = min[this.primaryAxes.Item1];

				Debug.DrawLine(p1, p2, Color.white, duration);
				Debug.DrawLine(p2, p3, Color.white, duration);
				Debug.DrawLine(p3, p4, Color.white, duration);
				Debug.DrawLine(p4, p1, Color.white, duration);
			}

			// Draw object bounds
			var drawnObjects = new HashSet<T>();
			foreach (var cellObjects in this.cells.Values)
			{
				foreach (var obj in cellObjects)
				{
					if (drawnObjects.Add(obj)) // Only draw each object once
					{
						var bounds = this.getBounds(obj);
						Vector3 min = bounds.min;
						Vector3 max = bounds.max;

						// Create four corners all at the same height (using min for the up axis)
						Vector3 p1 = Vector3.zero;
						Vector3 p2 = Vector3.zero;
						Vector3 p3 = Vector3.zero;
						Vector3 p4 = Vector3.zero;

						// Set the coordinates for the primary axes
						p1[this.primaryAxes.Item1] = min[this.primaryAxes.Item1];
						p1[this.primaryAxes.Item2] = min[this.primaryAxes.Item2];

						p2[this.primaryAxes.Item1] = max[this.primaryAxes.Item1];
						p2[this.primaryAxes.Item2] = min[this.primaryAxes.Item2];

						p3[this.primaryAxes.Item1] = max[this.primaryAxes.Item1];
						p3[this.primaryAxes.Item2] = max[this.primaryAxes.Item2];

						p4[this.primaryAxes.Item1] = min[this.primaryAxes.Item1];
						p4[this.primaryAxes.Item2] = max[this.primaryAxes.Item2];

						// Set the up axis coordinate to min for all points
						int upAxisIndex = (int)(this.upAxis) / 2; // Convert AxisDirection to index (0=X, 1=Y, 2=Z)
						float upCoord = min[upAxisIndex];
						p1[upAxisIndex] = upCoord;
						p2[upAxisIndex] = upCoord;
						p3[upAxisIndex] = upCoord;
						p4[upAxisIndex] = upCoord;

						Debug.DrawLine(p1, p2, Color.green, duration);
						Debug.DrawLine(p2, p3, Color.green, duration);
						Debug.DrawLine(p3, p4, Color.green, duration);
						Debug.DrawLine(p4, p1, Color.green, duration);
					}
				}
			}
		}
	}
}
