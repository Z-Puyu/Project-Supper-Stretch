using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	[AddComponentMenu("DunGen/Runtime Dungeon")]
	public class RuntimeDungeon : MonoBehaviour
	{
		public DungeonGenerator Generator = new DungeonGenerator();
		public bool GenerateOnStart = true;
		public GameObject Root;


		protected virtual void Start()
		{
			if (this.GenerateOnStart)
				this.Generate();
		}

		public void Generate()
		{
			if (this.Root != null)
				this.Generator.Root = this.Root;

			if (!this.Generator.IsGenerating)
				this.Generator.Generate();
		}

		private void OnDrawGizmos()
		{
			if (this.Generator == null || !this.Generator.DebugRender)
				return;

			this.Generator.CollisionManager?.Broadphase?.DrawDebug();
		}
	}
}
