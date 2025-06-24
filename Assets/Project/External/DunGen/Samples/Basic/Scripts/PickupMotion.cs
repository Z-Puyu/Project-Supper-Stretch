using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class PickupMotion : MonoBehaviour
	{
		public float SpinSpeed = 90;
		public float BobSpeed = 1;
		public float BobDistance = 1;

		private Vector3 positionOffset;


		protected virtual void Update()
		{
			this.transform.position -= this.positionOffset;
			this.positionOffset = this.transform.up * Mathf.Sin(Time.time * this.BobSpeed) * this.BobDistance;
			this.transform.position += this.positionOffset;

			this.transform.rotation *= Quaternion.AngleAxis(this.SpinSpeed * Time.deltaTime, this.transform.up);
		}
	}
}