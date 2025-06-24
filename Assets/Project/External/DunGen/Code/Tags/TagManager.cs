using System;
using System.Collections.Generic;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code.Tags
{
	[Serializable]
	public sealed class TagManager : ISerializationCallbackReceiver
	{
		public int TagCount { get { return this.tags.Count; } }

		private Dictionary<int, string> tags = new Dictionary<int, string>();

		[SerializeField]
		private List<int> keys = new List<int>();

		[SerializeField]
		private List<string> values = new List<string>();


		/// <summary>
		/// Attempt to get the name of a tag from its ID
		/// </summary>
		/// <param name="id">The ID of the tag to find</param>
		/// <returns>The name of the tag, or NULL if not found</returns>
		public string TryGetNameFromID(int id)
		{
			string name = null;
			this.tags.TryGetValue(id, out name);

			return name;
		}

		/// <summary>
		/// Check if a tag already exists with a specific name
		/// </summary>
		/// <param name="name">The name to look for</param>
		/// <param name="id">The ID of the existing tag of the same name, -1 otherwise</param>
		/// <returns>True if the tag exists</returns>
		public bool TagExists(string name, out int id)
		{
			foreach(var pair in this.tags)
			{
				if(pair.Value == name)
				{
					id = pair.Key;
					return true;
				}
			}

			id = -1;
			return false;
		}

		/// <summary>
		/// Attempt to rename a tag
		/// </summary>
		/// <param name="id">The ID of the tag to rename</param>
		/// <param name="newName">The desired new name</param>
		/// <returns>True if successful</returns>
		public bool TryRenameTag(int id, string newName)
		{
			string existingName;

			if (!this.tags.TryGetValue(id, out existingName))
				return false;

			if (existingName == newName)
				return true;

			int existingTagID;

			if (this.TagExists(newName, out existingTagID))
				return false;

			this.tags[id] = newName;
			return true;
		}

		/// <summary>
		/// Try to add a new tag with the given name
		/// </summary>
		/// <param name="tagName">The name given to the new tag</param>
		/// <returns>The ID of the new tag</returns>
		public int AddTag(string tagName)
		{
			tagName = this.GetUnusedTagName(tagName);
			int newID = 0;

			foreach (var id in this.tags.Keys)
				newID = Mathf.Max(newID, id + 1);

			this.tags[newID] = tagName;
			return newID;
		}

		/// <summary>
		/// Gets an unused tag name. If the desired tag name is already
		/// in use, a number is affixed to make it unique
		/// </summary>
		/// <param name="desiredTagName">The ideal tag name to use</param>
		/// <returns>A unique tag name that's not already in use</returns>
		private string GetUnusedTagName(string desiredTagName)
		{
			bool nameExists = false;

			foreach (var pair in this.tags)
			{
				if (pair.Value == desiredTagName)
				{
					nameExists = true;
					break;
				}
			}

			if (!nameExists)
				return desiredTagName;


			int affix = 2;
			string newTagName = desiredTagName + " " + affix;

			int existingTagID;

			while (this.TagExists(newTagName, out existingTagID))
			{
				newTagName = desiredTagName + " " + affix;
				affix++;
			}

			return newTagName;
		}

		/// <summary>
		/// Remove a tag
		/// </summary>
		/// <param name="id">The ID of the tag to remove</param>
		/// <returns>True if the tag was successfuly removed</returns>
		public bool RemoveTag(int id)
		{
			if (!this.tags.ContainsKey(id))
				return false;

			this.tags.Remove(id);
			return true;
		}

		/// <summary>
		/// Get a list of all of the available tags
		/// </summary>
		/// <returns>The ID of every available tag</returns>
		public int[] GetTagIDs()
		{
			var tagIDs = new int[this.tags.Count];
			int index = 0;

			foreach (var id in this.tags.Keys)
			{
				tagIDs[index] = id;
				index++;
			}

			Array.Sort(tagIDs);
			return tagIDs;
		}

		#region ISerializationCallbackReceiver

		public void OnAfterDeserialize()
		{
			this.tags = new Dictionary<int, string>();

			for (int i = 0; i < this.keys.Count; i++)
				this.tags[this.keys[i]] = this.values[i];

			this.keys.Clear();
			this.values.Clear();
		}

		public void OnBeforeSerialize()
		{
			this.keys = new List<int>();
			this.values = new List<string>();

			foreach(var pair in this.tags)
			{
				this.keys.Add(pair.Key);
				this.values.Add(pair.Value);
			}	
		}

		#endregion
	}
}
