using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DunGen.Project.External.DunGen.Code
{
	[Serializable]
	[CreateAssetMenu(menuName = "DunGen/Key Manager", order = 700)]
	public sealed class KeyManager : ScriptableObject
	{
		public ReadOnlyCollection<Key> Keys
		{
			get
			{
				if (this.keysReadOnly == null)
					this.keysReadOnly = new ReadOnlyCollection<Key>(this.keys);

				return this.keysReadOnly;
			}
		}

		private ReadOnlyCollection<Key> keysReadOnly;

		[SerializeField]
		private List<Key> keys = new List<Key>();


		public Key CreateKey()
		{
			Key key = new Key(this.GetNextAvailableID());
			key.Name = UnityUtil.GetUniqueName("New Key", this.keys.Select(x => x.Name));
			key.Colour = new Color(Random.value, Random.value, Random.value);

			this.keys.Add(key);

			return key;
		}

		public void DeleteKey(int index)
		{
			this.keys.RemoveAt(index);
		}

		public Key GetKeyByID(int id)
		{
			return this.keys.Where(x => { return x.ID == id; }).FirstOrDefault();
		}

		public Key GetKeyByName(string name)
		{
			return this.keys.Where(x => { return x.Name == name; }).FirstOrDefault();
		}

		public bool RenameKey(int index, string newName)
		{
			if(this.keys[index].Name == newName)
				return false;

			newName = UnityUtil.GetUniqueName(newName, this.keys.Select(x => x.Name));

			this.keys[index].Name = newName;
			return true;
		}

		private int GetNextAvailableID()
		{
			int nextID = 0;

			foreach(var key in this.keys.OrderBy(x => x.ID))
			{
				if(key.ID >= nextID)
					nextID = key.ID + 1;
			}

			return nextID;
		}
	}
}

