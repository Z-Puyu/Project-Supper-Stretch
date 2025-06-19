using System.Linq;
using System.Text;
using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerController : MonoBehaviour
	{
		public float MinYaw = -360;
		public float MaxYaw = 360;
		public float MinPitch = -60;
		public float MaxPitch = 60;
		public float LookSensitivity = 1;

		public float MoveSpeed = 10;
		public float TurnSpeed = 90;

		public bool IsControlling { get { return this.isControlling; } }
		public Camera ActiveCamera { get { return this.isControlling ? this.playerCamera : this.overheadCamera; } }

		protected CharacterController movementController;
		protected Camera playerCamera;
		protected Camera overheadCamera;
		protected bool isControlling;
		protected float yaw;
		protected float pitch;
		protected Generator gen;
		protected Vector3 velocity;


		protected virtual void Start()
		{
			this.movementController = this.GetComponent<CharacterController>();
			this.playerCamera = this.GetComponentInChildren<Camera>();
			this.gen = UnityUtil.FindObjectByType<Generator>();
			this.overheadCamera = GameObject.Find("Overhead Camera").GetComponent<Camera>();

			this.isControlling = true;
			this.ToggleControl();

			this.gen.DungeonGenerator.Generator.OnGenerationStatusChanged += this.OnGenerationStatusChanged;
			this.gen.GetAdditionalText = this.GetAdditionalScreenText;
		}

		protected virtual void OnDestroy()
		{
			this.gen.DungeonGenerator.Generator.OnGenerationStatusChanged -= this.OnGenerationStatusChanged;
			this.gen.GetAdditionalText = null;
		}

		private void GetAdditionalScreenText(StringBuilder infoText)
		{
			infoText.AppendLine("Press 'C' to switch between camera modes");
		}

		protected virtual void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			if (status == GenerationStatus.Complete)
			{
				this.FrameDungeonWithCamera();
				this.transform.position = new Vector3(0, 1, 7); // Hard-coded spawn position
				this.velocity = Vector3.zero;
			}
		}

		protected virtual void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
				this.ToggleControl();

			// Repeatedly frame the dungeon while the generation process is running
			var generator = this.gen.DungeonGenerator.Generator;
			if (generator.IsGenerating && generator.GenerateAsynchronously && generator.PauseBetweenRooms > 0f)
				this.FrameDungeonWithCamera();

			if (this.isControlling)
			{
				Vector3 direction = Vector3.zero;
				direction += this.transform.forward * Input.GetAxisRaw("Vertical");
				direction += this.transform.right * Input.GetAxisRaw("Horizontal");

				direction.Normalize();

				if (this.movementController.isGrounded)
					this.velocity = Vector3.zero;
				else
					this.velocity += -this.transform.up * (9.81f * 10) * Time.deltaTime; // Gravity

				direction += this.velocity * Time.deltaTime;
				this.movementController.Move(direction * Time.deltaTime * this.MoveSpeed);

				// Camera Look
				this.yaw += Input.GetAxisRaw("Mouse X") * this.LookSensitivity;
				this.pitch += Input.GetAxisRaw("Mouse Y") * this.LookSensitivity;

				this.yaw = this.ClampAngle(this.yaw, this.MinYaw, this.MaxYaw);
				this.pitch = this.ClampAngle(this.pitch, this.MinPitch, this.MaxPitch);

				this.transform.rotation = Quaternion.AngleAxis(this.yaw, Vector3.up);
				this.playerCamera.transform.localRotation = Quaternion.AngleAxis(this.pitch, -Vector3.right);
			}
		}

		protected float ClampAngle(float angle)
		{
			return this.ClampAngle(angle, 0, 360);
		}

		protected float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;

			return Mathf.Clamp(angle, min, max);
		}

		protected void ToggleControl()
		{
			this.isControlling = !this.isControlling;

			this.overheadCamera.gameObject.SetActive(!this.isControlling);
			this.playerCamera.gameObject.SetActive(this.isControlling);

			this.overheadCamera.transform.position = new Vector3(this.transform.position.x, this.overheadCamera.transform.position.y, this.transform.position.z);

			Cursor.lockState = (this.isControlling) ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !this.isControlling;

			if (!this.isControlling)
				this.FrameDungeonWithCamera();
		}

		protected void FrameDungeonWithCamera()
		{
			var allDungeons = UnityUtil.FindObjectsByType<Dungeon>()
				.Select(x => x.gameObject)
				.ToArray();

			this.FrameObjectsWithCamera(allDungeons);
		}

		protected void FrameObjectsWithCamera(params GameObject[] gameObjects)
		{
			if (gameObjects == null || gameObjects.Length == 0)
				return;

			bool hasBounds = false;
			Bounds bounds = new Bounds();

			foreach(var obj in gameObjects)
			{
				var objBounds = UnityUtil.CalculateObjectBounds(obj, false, false);

				if (!hasBounds)
				{
					bounds = objBounds;
					hasBounds = true;
				}
				else
					bounds.Encapsulate(objBounds);
			}

			if (!hasBounds)
				return;

			float radius = Mathf.Max(bounds.size.x, bounds.size.z);

			float distance = radius / Mathf.Sin(this.overheadCamera.fieldOfView / 2);
			distance = Mathf.Abs(distance);

			Vector3 position = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);
			position += this.gen.DungeonGenerator.Generator.UpVector * distance;

			this.overheadCamera.transform.position = position;
		}
	}
}