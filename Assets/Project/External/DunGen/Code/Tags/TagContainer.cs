using System;
using System.Collections;
using System.Collections.Generic;

namespace DunGen.Project.External.DunGen.Code.Tags
{
	[Serializable]
	public sealed class TagContainer : IEnumerable<Tag>
	{
		public List<Tag> Tags = new List<Tag>();


		public TagContainer()
		{
		}

		public TagContainer(TagContainer other)
		{
			if (other == null || other.Tags == null)
				return;

			foreach (var tag in other.Tags)
				this.Tags.Add(new Tag(tag.ID));
		}

		public bool HasTag(Tag tag)
		{
			return this.Tags.Contains(tag);
		}

		public bool HasAnyTag(params Tag[] tags)
		{
			foreach (var tag in tags)
				if (this.HasTag(tag))
					return true;

			return false;
		}

		public bool HasAnyTag(TagContainer tags)
		{
			foreach (var tag in tags)
				if (this.HasTag(tag))
					return true;

			return false;
		}

		public bool HasAllTags(params Tag[] tags)
		{
			bool hasAllTags = true;

			foreach (var tag in tags)
			{
				if (!this.HasTag(tag))
				{
					hasAllTags = false;
					break;
				}
			}

			return hasAllTags;
		}

		public bool HasAllTags(TagContainer tags)
		{
			bool hasAllTags = true;

			foreach (var tag in tags)
			{
				if (!this.HasTag(tag))
				{
					hasAllTags = false;
					break;
				}
			}

			return hasAllTags;
		}

		#region IEnumerable<Tag>

		public IEnumerator<Tag> GetEnumerator()
		{
			return this.Tags.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Tags.GetEnumerator();
		}

		#endregion
	}
}
