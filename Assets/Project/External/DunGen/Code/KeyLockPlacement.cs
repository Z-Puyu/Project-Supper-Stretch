using System;
using DunGen.Project.External.DunGen.Code.Utility;

namespace DunGen.Project.External.DunGen.Code
{
	[Serializable]
	public sealed class KeyLockPlacement
	{
		public int ID;
		public IntRange Range = new IntRange(0, 1);
	}
}

