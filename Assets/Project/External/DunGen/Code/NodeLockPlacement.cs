using System;

namespace DunGen
{
    [Flags]
	public enum NodeLockPlacement
	{
        Entrance    = 0x01,
        Exit        = 0x02,
	}
}
