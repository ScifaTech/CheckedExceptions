namespace Scifa.CheckedExceptions.Test
{
	public static class StringHelpers
	{
		public static int LineCount(this string s)
		{
			return CharCount(s, '\n');
		}

		private static int CharCount(this string s, char v)
		{
			int result = 0;
			for (int i = 0; i < s.Length; i++)
				if (s[i] == v) result++;

			return result;
		}
	}
}
