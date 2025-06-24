using DunGen.Project.External.DunGen.Code;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class KeyColour : MonoBehaviour, IKeyLock
	{
		[SerializeField]
		private int keyID;

		[SerializeField]
		private KeyManager keyManager;

		private MaterialPropertyBlock propertyBlock;


		public void OnKeyAssigned(Key key, KeyManager manager)
		{
			this.keyID = key.ID;
			this.keyManager = manager;

			this.SetColour(key.Colour);
		}

		private void Start()
		{
			if (this.keyManager == null)
				return;

			var key = this.keyManager.GetKeyByID(this.keyID);
			this.SetColour(key.Colour);
		}

		private void SetColour(Color colour)
		{
			if (Application.isPlaying)
			{
				if(this.propertyBlock == null)
					this.propertyBlock = new MaterialPropertyBlock();

				this.propertyBlock.SetColor("_Color", colour);

				foreach (var r in this.GetComponentsInChildren<Renderer>())
					r.SetPropertyBlock(this.propertyBlock);
			}
		}
	}
}