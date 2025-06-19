using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.LockAndKey;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class KeySpawnPoint : MonoBehaviour, IKeySpawner
	{
		public bool SetColourOnSpawn = true;

		private MaterialPropertyBlock propertyBlock;
		private GameObject spawnedKey;


		#region IKeySpawner Implementation

		public bool CanSpawnKey(KeyManager keyManaager, Key key)
		{
			// Has already spawned a key (this check shouldn't be necessary)
			if (this.spawnedKey != null)
				return false;

			// Cannot spawn a key that doesn't have a prefab
			return key.Prefab != null;
		}

		public void SpawnKey(KeySpawnParameters keySpawnParameters)
		{
			// Spawn the key attached to the dungeon root
			this.spawnedKey = GameObject.Instantiate(keySpawnParameters.Key.Prefab);
			this.spawnedKey.transform.parent = keySpawnParameters.DungeonGenerator.Root.transform;
			this.spawnedKey.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);

			if (this.SetColourOnSpawn && Application.isPlaying)
			{
				if (this.propertyBlock == null)
					this.propertyBlock = new MaterialPropertyBlock();

				this.propertyBlock.SetColor("_Color", keySpawnParameters.Key.Colour);

				foreach (var r in this.spawnedKey.GetComponentsInChildren<Renderer>())
					r.SetPropertyBlock(this.propertyBlock);
			}

			// Pass any components that implement IKeyLock back to the dungeon generator
			keySpawnParameters.OutputSpawnedKeys.AddRange(this.spawnedKey.GetComponents<IKeyLock>());
		}

		#endregion
	}
}