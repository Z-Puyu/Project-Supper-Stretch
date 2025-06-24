using System.Text.RegularExpressions;

namespace DunGen.Project.External.DunGen.Code.Utility
{
	public static class StringUtil
	{
		private static Regex capitalLetterPattern = new Regex("([A-Z])");


		public static string SplitCamelCase(string input)
		{
			return StringUtil.capitalLetterPattern.Replace(input, " $1").Trim();
		}
	}
}
