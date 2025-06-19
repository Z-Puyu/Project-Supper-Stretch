using System.Collections.Generic;
using DunGen.Project.External.DunGen.Code.Pooling;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	[AddComponentMenu("DunGen/Random Props/Random Prefab")]
	public class RandomPrefab : RandomProp, ITileSpawnEventReceiver
	{
		[AcceptGameObjectTypes(GameObjectFilter.Asset)]
		public GameObjectChanceTable Props = new GameObjectChanceTable();
		public bool ZeroPosition = true;
		public bool ZeroRotation = true;

		private GameObject propInstance;


		private void ClearExistingInstances()
		{
			if (this.propInstance == null)
				return;

			Object.DestroyImmediate(this.propInstance);
			this.propInstance = null;
		}

		public override void Process(RandomStream randomStream, Tile tile, ref List<GameObject> spawnedObjects)
		{
			this.ClearExistingInstances();

			if (this.Props.Weights.Count <= 0)
				return;

			var chosenEntry = this.Props.GetRandom(randomStream,
				tile.Placement.IsOnMainPath,
				tile.Placement.NormalizedDepth,
				previouslyChosen: null,
				allowImmediateRepeats: true,
				removeFromTable: false,
				allowNullSelection: true);

			if (chosenEntry == null || chosenEntry.Value == null)
				return;

			var prefab = chosenEntry.Value;

			this.propInstance = Object.Instantiate(prefab);
			this.propInstance.transform.parent = this.transform;

			spawnedObjects.Add(this.propInstance);

			if (this.ZeroPosition)
				this.propInstance.transform.localPosition = Vector3.zero;
			else
				this.propInstance.transform.localPosition = prefab.transform.localPosition;

			if (this.ZeroRotation)
				this.propInstance.transform.localRotation = Quaternion.identity;
			else
				this.propInstance.transform.localRotation = prefab.transform.localRotation;
		}

		//
		// Begin ITileSpawnEventReceiver implementation

		public void OnTileSpawned(Tile tile) { }

		public void OnTileDespawned(Tile tile) => this.ClearExistingInstances();

		// End ITileSpawnEventReceiver implementation
		//
	}
}