using System.Collections.Generic;
using DunGen.Project.External.DunGen.Code;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class DemoTileInjector : MonoBehaviour
	{
		public RuntimeDungeon RuntimeDungeon;
		public TileSet TileSet;
		public float NormalizedPathDepth;
		public float NormalizedBranchDepth;
		public bool IsOnMainPath;


		private void Awake()
		{
			this.RuntimeDungeon.Generator.TileInjectionMethods += this.InjectTiles;
		}

		private void InjectTiles(RandomStream randomStream, ref List<InjectedTile> tilesToInject)
		{
			tilesToInject.Add(new InjectedTile(this.TileSet, this.IsOnMainPath, this.NormalizedPathDepth, this.NormalizedBranchDepth));
		}
	}
}
