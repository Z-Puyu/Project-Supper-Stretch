using UnityEngine;

namespace DunGen.Project.External.DunGen.Code.Adapters
{
	public abstract class BaseAdapter : MonoBehaviour
	{
		public int Priority = 0;
		public virtual bool RunDuringAnalysis { get; set; }

		protected DungeonGenerator dungeonGenerator;


		protected virtual void OnEnable()
		{
			var runtimeDungeon = this.GetComponent<RuntimeDungeon>();

			if (runtimeDungeon != null)
			{
				this.dungeonGenerator = runtimeDungeon.Generator;
				this.dungeonGenerator.RegisterPostProcessStep(this.OnPostProcess, this.Priority);
				this.dungeonGenerator.Cleared += this.Clear;
			}
			else
				Debug.LogError("[DunGen Adapter] RuntimeDungeon component is missing on GameObject '" + this.gameObject.name + "'. Adapters must be attached to the same GameObject as your RuntimeDungeon component");
		}

		protected virtual void OnDisable()
		{
			if (this.dungeonGenerator != null)
			{
				this.dungeonGenerator.UnregisterPostProcessStep(this.OnPostProcess);
				this.dungeonGenerator.Cleared -= this.Clear;
			}
		}

		private void OnPostProcess(DungeonGenerator generator)
		{
			if (!generator.IsAnalysis || this.RunDuringAnalysis)
				this.Run(generator);
		}

		protected virtual void Clear() { }
		protected abstract void Run(DungeonGenerator generator);
	}
}
