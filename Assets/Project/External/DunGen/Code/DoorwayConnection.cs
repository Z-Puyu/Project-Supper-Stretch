using System;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	[Serializable]
	public sealed class DoorwayConnection
	{
		public Doorway A => this.a;
		public Doorway B => this.b;

		[SerializeField]
		private Doorway a;
		[SerializeField]
		private Doorway b;

		public DoorwayConnection(Doorway a, Doorway b)
		{
			this.a = a;
			this.b = b;
		}
	}
}