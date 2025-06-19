using System.Collections.Generic;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class PlayerInventory : MonoBehaviour
	{
		private List<int> keys = new List<int>();


		public bool HasKey(int keyID)
		{
			return this.keys.Contains(keyID);
		}

		public void AddKey(int keyID)
		{
			this.keys.Add(keyID);
		}

		public void RemoveKey(int keyID)
		{
			this.keys.Remove(keyID);
		}
	}
}