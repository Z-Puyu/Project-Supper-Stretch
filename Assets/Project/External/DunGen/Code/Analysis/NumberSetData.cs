using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code.Analysis
{
	public sealed class NumberSetData
	{
		public float Min { get; private set; }
		public float Max { get; private set; }
		public float Average { get; private set; }
		public float StandardDeviation { get; private set; }


		public NumberSetData(IEnumerable<float> values)
		{
			this.Min = values.Min();
			this.Max = values.Max();
			this.Average = values.Sum() / values.Count();

			// Calculate standard deviation
			float[] temp = new float[values.Count()];

			for (int i = 0; i < temp.Length; i++)
				temp[i] = Mathf.Pow(values.ElementAt(i) - this.Average, 2);

			this.StandardDeviation = Mathf.Sqrt(temp.Sum() / temp.Length);
		}

		public override string ToString()
		{
			return string.Format("[ Min: {0:N1}, Max: {1:N1}, Average: {2:N1}, Standard Deviation: {3:N2} ]", this.Min, this.Max, this.Average, this.StandardDeviation);
		}
	}
}

