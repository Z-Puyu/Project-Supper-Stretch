using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class CameraMovement : MonoBehaviour
	{
		public float MovementSpeed = 100;


		private void Start()
		{
			var runtimeDungeon = UnityUtil.FindObjectByType<RuntimeDungeon>();

			if (runtimeDungeon != null)
				this.transform.forward = -runtimeDungeon.Generator.UpVector;
		}

		private void Update()
		{
			Vector3 direction = Vector3.zero;

			direction += this.transform.up * Input.GetAxisRaw("Vertical");
			direction += this.transform.right * Input.GetAxisRaw("Horizontal");

			direction.Normalize();

			Vector3 offset = direction * this.MovementSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.LeftShift))
				offset *= 2;

			float zoom = Input.GetAxisRaw("Mouse ScrollWheel");
			offset += this.transform.forward * zoom * Time.deltaTime * this.MovementSpeed * 100;

			this.transform.position += offset;
		}
	}
}