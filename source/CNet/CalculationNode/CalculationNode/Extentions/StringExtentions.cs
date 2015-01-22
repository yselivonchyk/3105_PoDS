namespace CalculationNode.Extentions
{
	public static class StringExtentions
	{
		public static bool GrossPlatformCompare(this string self, string arg)
		{
			if (self.Length != arg.Length)
				return self.Length > arg.Length;
			for (int i = 0; i < arg.Length; i++)
			{
				if (self[i] == arg[i])
					continue;
				return self[i] > arg[i];
			}
			return false;
		}
	}
}
