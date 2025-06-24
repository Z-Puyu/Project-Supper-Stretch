using DunGen.Project.External.DunGen.Code;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class LockedDoor : MonoBehaviour, IKeyLock
	{
		public Key Key { get { return this.keyManager.GetKeyByID(this.keyID); } }
		public float OpenDuration = 1.0f;
		public Vector3 OpenPositionOffset = new Vector3(0, -3, 0);

		[HideInInspector]
		[SerializeField]
		private int keyID;

		[HideInInspector]
		[SerializeField]
		private KeyManager keyManager;

		private Vector3 initialPosition;
		private float openTime;
		private bool isOpening;
		private Door door;


		private void Start()
		{
			this.door = this.GetComponent<Door>();
		}

		public void OnKeyAssigned(Key key, KeyManager keyManager)
		{
			this.keyID = key.ID;
			this.keyManager = keyManager;
		}

		private void OnTriggerEnter(Collider c)
		{
			if (this.isOpening)
				return;

			var inventory = c.GetComponent<PlayerInventory>();

			if (inventory == null)
				return;

			if (inventory.HasKey(this.keyID))
			{
				ScreenText.Log("Opened {0} door", this.Key.Name);

				inventory.RemoveKey(this.keyID);
				this.Open();
			}
			else
				ScreenText.Log("{0} key required", this.Key.Name);
		}

		private void Update()
		{
			if (this.isOpening)
			{
				this.openTime += Time.deltaTime;

				if (this.openTime >= this.OpenDuration)
				{
					this.openTime = this.OpenDuration;
					this.isOpening = false;
				}

				this.transform.position = Vector3.Lerp(this.initialPosition, this.initialPosition + this.OpenPositionOffset, this.openTime / this.OpenDuration);
			}
		}

		private void Open()
		{
			if (this.isOpening)
				return;

			this.isOpening = true;
			this.initialPosition = this.transform.position;
			this.openTime = 0;
			this.door.IsOpen = true;
		}
	}
}