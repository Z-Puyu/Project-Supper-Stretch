using System;

namespace DunGen.Project.External.DunGen.Code.Utility
{
	/**
     * A series of classes for getting a random value between a given range
     */

	[Serializable]
	public class IntRange
	{
		public int Min;
		public int Max;


		public IntRange() { }
		public IntRange(int min, int max)
		{
			this.Min = min;
			this.Max = max;
		}

		public int GetRandom(RandomStream random)
		{
			if (this.Min > this.Max)
				this.Max = this.Min;

			return random.Next(this.Min, this.Max + 1);
		}

		public override string ToString()
		{
			return this.Min + " - " + this.Max;
		}
	}

	[Serializable]
	public class FloatRange
	{
		public float Min;
		public float Max;


		public FloatRange() { }
		public FloatRange(float min, float max)
		{
			this.Min = min;
			this.Max = max;
		}

		public float GetRandom(RandomStream random)
		{
			if (this.Min > this.Max)
			{
				float temp = this.Min;
				this.Min = this.Max;
				this.Max = temp;
			}

			float range = this.Max - this.Min;
			return this.Min + ((float)random.NextDouble() * range);
		}
	}
}
