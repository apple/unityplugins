using System.Linq;
using System.Text.RegularExpressions;

namespace UnityPickers.Utility
{
	public static class StringUtils
	{
		public static bool MatchesFilter(string text, string filter)
		{
			var elems = filter
				.ToLowerInvariant()
				.Split()
				.Select(Regex.Escape)
				.ToArray();

			var pattern = string.Join(".*", elems);
			return Regex.Match(text.ToLowerInvariant(), pattern).Success;
		}
	}
}