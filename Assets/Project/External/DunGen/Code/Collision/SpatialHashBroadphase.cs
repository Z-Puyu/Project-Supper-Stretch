using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code.Collision
{
	[Serializable]
	[DisplayName("Spatial Hashing")]
	public class SpatialHashBroadphaseSettings : BroadphaseSettings
	{
		public float CellSize = 40.0f;

		public override ICollisionBroadphase Create()
		{
			return new SpatialHashBroadphase();
		}
	}

	public class SpatialHashBroadphase : ICollisionBroadphase
	{
		public SpatialHashGrid<Bounds> SpatialHashGrid { get; private set; }


		public void Init(BroadphaseSettings settings, DungeonGenerator dungeonGenerator)
		{
			if (!(settings is SpatialHashBroadphaseSettings spatialHashSettings))
				return;

			this.SpatialHashGrid = new SpatialHashGrid<Bounds>(
				spatialHashSettings.CellSize,
				(b) => b,
				dungeonGenerator.UpDirection);
		}

		public void Insert(Bounds bounds)
		{
			this.SpatialHashGrid.Insert(bounds);
		}

		public void Query(Bounds bounds, ref List<Bounds> results)
		{
			this.SpatialHashGrid.Query(bounds, ref results);
		}

		public void Remove(Bounds bounds)
		{
			this.SpatialHashGrid.Remove(bounds);
		}

		public void DrawDebug(float duration = 0)
		{
			this.SpatialHashGrid.DrawDebug(duration);
		}
	}
}
