using System;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	[Serializable]
	public sealed class GameObjectWeight
	{
		public GameObject GameObject = null;
		public float Weight = 1f;


		public GameObjectWeight()
		{
		}

		public GameObjectWeight(GameObject gameObject, float weight = 1f)
		{
			this.GameObject = gameObject;
			this.Weight = weight;
		}
	}
}
