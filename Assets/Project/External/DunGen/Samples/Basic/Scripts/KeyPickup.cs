using DunGen.Project.External.DunGen.Code;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class KeyPickup : MonoBehaviour, IKeyLock
	{
		public Key Key { get { return this.keyManager.GetKeyByID(this.keyID); } }

		[HideInInspector]
		[SerializeField]
		private int keyID;

		[HideInInspector]
		[SerializeField]
		private KeyManager keyManager;


		public void OnKeyAssigned(Key key, KeyManager keyManager)
		{
			this.keyID = key.ID;
			this.keyManager = keyManager;
		}

		private void OnTriggerEnter(Collider c)
		{
			var inventory = c.GetComponent<PlayerInventory>();

			if (inventory == null)
				return;

			ScreenText.Log("Picked up {0} key", this.Key.Name);
			inventory.AddKey(this.keyID);
			Object.Destroy(this.gameObject);
		}
	}
}