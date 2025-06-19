using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace Project.External.DunGen.Samples.Simple_2D.Scripts
{
	sealed class PlayerController2D : MonoBehaviour
	{
		public float MovementSpeed = 6f;

		private new CircleCollider2D collider;
		private RaycastHit2D[] hitBuffer;
		private DungeonGenerator dungeonGenerator;


		private void Start()
		{
			this.collider = this.GetComponent<CircleCollider2D>();
			this.hitBuffer = new RaycastHit2D[10];

			var gen = UnityUtil.FindObjectByType<global::Project.External.DunGen.Samples.Basic.Scripts.Generator>();
			this.dungeonGenerator = gen.DungeonGenerator.Generator;

			this.dungeonGenerator.OnGenerationStatusChanged += this.OnGeneratorStatusChanged;
		}

		private void OnDestroy()
		{
			this.dungeonGenerator.OnGenerationStatusChanged -= this.OnGeneratorStatusChanged;
		}

		private void OnGeneratorStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			this.transform.position = Vector3.zero;
		}

		private void Update()
		{
			Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			if(input.sqrMagnitude > 1f)
				input.Normalize();

			Vector3 direction = new Vector3(input.x, input.y, 0f);
			float distance = this.MovementSpeed * Time.deltaTime;
			Vector3 motion = direction * distance;

			int hitCount = this.collider.Cast(direction, this.hitBuffer, distance);

			if (hitCount > 0)
			{
				var hit = this.hitBuffer[0];
				motion = direction * hit.distance;
			}

			this.transform.position += motion;
		}
	}
}
