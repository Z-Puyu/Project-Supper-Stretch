using System;

namespace DunGen.Project.External.DunGen.Code
{
	public sealed class RandomStream
	{
		public static readonly RandomStream Global = new RandomStream();

		private const int maxValue = int.MaxValue;
		private const int seed = 161803398;

		private int iNext;
		private int iNextP;
		private int[] seedArray = new int[56];


		public RandomStream()
		  : this(Environment.TickCount)
		{
		}

		public RandomStream(int Seed)
		{
			int ii;
			int mj, mk;

			int subtraction = (Seed == int.MinValue) ? int.MaxValue : Math.Abs(Seed);
			mj = RandomStream.seed - subtraction;
			this.seedArray[55] = mj;
			mk = 1;

			for (int i = 1; i < 55; i++)
			{
				ii = (21 * i) % 55;
				this.seedArray[ii] = mk;
				mk = mj - mk;

				if (mk < 0)
					mk += RandomStream.maxValue;

				mj = this.seedArray[ii];
			}

			for (int k = 1; k < 5; k++)
			{
				for (int i = 1; i < 56; i++)
				{
					this.seedArray[i] -= this.seedArray[1 + (i + 30) % 55];

					if (this.seedArray[i] < 0)
						this.seedArray[i] += RandomStream.maxValue;
				}
			}

			this.iNext = 0;
			this.iNextP = 21;
			Seed = 1;
		}

		private double Sample()
		{
			return (this.InternalSample() * (1.0 / RandomStream.maxValue));
		}

		private int InternalSample()
		{
			int retVal;
			int locINext = this.iNext;
			int locINextp = this.iNextP;

			if (++locINext >= 56)
				locINext = 1;

			if (++locINextp >= 56)
				locINextp = 1;

			retVal = this.seedArray[locINext] - this.seedArray[locINextp];

			if (retVal == RandomStream.maxValue)
				retVal--;

			if (retVal < 0)
				retVal += RandomStream.maxValue;

			this.seedArray[locINext] = retVal;

			this.iNext = locINext;
			this.iNextP = locINextp;

			return retVal;
		}

		/// <summary>
		/// Returns a random integer between 0 (inclusive) and int.MaxValue (exclusive)
		/// </summary>
		/// <returns>A random integer between 0 (inclusive) and int.MaxValue (exclusive)</returns>
		public int Next()
		{
			return this.InternalSample();
		}

		private double GetSampleForLargeRange()
		{
			int result = this.InternalSample();

			bool negative = (this.InternalSample() % 2 == 0) ? true : false;

			if (negative)
				result = -result;

			double d = result;
			d += (int.MaxValue - 1);
			d /= 2 * (uint)int.MaxValue - 1;

			return d;
		}

		/// <summary>
		/// Returns a random integer between minValue (inclusive) and maxValue (exclusive)
		/// </summary>
		/// <param name="minValue">Inclusive min value</param>
		/// <param name="maxValue">Exclusive max value</param>
		/// <returns>A random integer between minValue (inclusive) and maxValue (exclusive)</returns>
		/// <exception cref="ArgumentOutOfRangeException">minValue must be greater than maxValue</exception>
		public int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
				throw new ArgumentOutOfRangeException("minValue");

			long range = (long)maxValue - minValue;

			if (range <= (long)Int32.MaxValue)
				return ((int)(this.Sample() * range) + minValue);
			else
				return (int)((long)(this.GetSampleForLargeRange() * range) + minValue);
		}

		/// <summary>
		/// Returns a non-negative random integer that is less than the specified maximum.
		/// </summary>
		/// <param name="maxValue">Exclusive maximum</param>
		/// <returns>Random integer between 0 and maxValue (exclusive)</returns>
		/// <exception cref="ArgumentOutOfRangeException">Max value < 0</exception>
		public int Next(int maxValue)
		{
			if (maxValue < 0)
				throw new ArgumentOutOfRangeException("maxValue");

			return (int)(this.Sample() * maxValue);
		}

		/// <summary>
		/// Returns a random double between 0.0 (inclusive) and 1.0 (exclusive)
		/// </summary>
		/// <returns>A random double between 0.0 (inclusive) and 1.0 (exclusive)</returns>
		public double NextDouble()
		{
			return this.Sample();
		}

		/// <summary>
		/// Fills an array with random bytes between 0 and 255 (inclusive)
		/// </summary>
		/// <param name="buffer">The array to fill</param>
		/// <exception cref="ArgumentNullException">`buffer` must not be null</exception>
		public void NextBytes(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");

			for (int i = 0; i < buffer.Length; i++)
				buffer[i] = (byte)(this.InternalSample() % (byte.MaxValue + 1));
		}
	}
}