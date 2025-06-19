using DunGen.Project.External.DunGen.Code;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class AutoDoor : MonoBehaviour
	{
		public enum DoorState
		{
			Open,
			Closed,
			Opening,
			Closing,
		}

		public GameObject Door;
		public Vector3 OpenOffset = new Vector3(0, 2.5f, 0);
		public float Speed = 3.0f;

		private Vector3 closedPosition;
		private DoorState currentState = DoorState.Closed;
		private float currentFramePosition = 0.0f;
		private Door doorComponent;


		private void Start()
		{
			this.doorComponent = this.GetComponent<Door>();
			this.closedPosition = this.Door.transform.localPosition;
		}

		private void Update()
		{
			if (this.currentState == DoorState.Opening || this.currentState == DoorState.Closing)
			{
				Vector3 openPosition = this.closedPosition + this.OpenOffset;

				float frameOffset = this.Speed * Time.deltaTime;

				if (this.currentState == DoorState.Closing)
					frameOffset *= -1;

				this.currentFramePosition += frameOffset;
				this.currentFramePosition = Mathf.Clamp(this.currentFramePosition, 0, 1);

				this.Door.transform.localPosition = Vector3.Lerp(this.closedPosition, openPosition, this.currentFramePosition);

				// Finished
				if (this.currentFramePosition == 1.0f)
					this.currentState = DoorState.Open;
				else if (this.currentFramePosition == 0.0f)
				{
					this.currentState = DoorState.Closed;
					this.doorComponent.IsOpen = false;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			PlayerController playerController = other.GetComponent<PlayerController>();

			// Ignore overlaps with anything other than the player
			if (playerController == null)
				return;

			this.currentState = DoorState.Opening;
			this.doorComponent.IsOpen = true;
		}

		private void OnTriggerExit(Collider other)
		{
			PlayerController playerController = other.GetComponent<PlayerController>();

			// Ignore overlaps with anything other than the player
			if (playerController == null)
				return;

			this.currentState = DoorState.Closing;
		}
	}
}