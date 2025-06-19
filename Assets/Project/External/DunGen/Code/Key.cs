using System;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	[Serializable]
	public sealed class Key
	{
		public int ID
		{
			get { return this.id; }
			set { this.id = value; }
		}
		public string Name
		{
			get { return this.name; }
			internal set { this.name = value; }
		}
		public GameObject Prefab;
		public Color Colour;
        public IntRange KeysPerLock = new IntRange(1, 1);


		[SerializeField]
		private int id;
		[SerializeField]
		private string name;


		internal Key(int id)
		{
			this.id = id;
		}
	}
}

