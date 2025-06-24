using System;

namespace DunGen.Project.External.DunGen.Code.Tags
{
	[Serializable]
	public sealed class TagPair
	{
		public Tag TagA;
		public Tag TagB;


		public TagPair()
		{
		}

		public TagPair(Tag a, Tag b)
		{
			this.TagA = a;
			this.TagB = b;
		}

		public override string ToString()
		{
			return string.Format("{0} <-> {1}", this.TagA.Name, this.TagB.Name);
		}

		public bool Matches(Tag a, Tag b, bool twoWay)
		{
			if (twoWay)
				return (a == this.TagA && b == this.TagB) || (a == this.TagB && b == this.TagA);
			else
				return a == this.TagA && b == this.TagB;
		}
	}
}
