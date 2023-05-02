namespace Pandority
{
	using System;

	internal static class DebugSwitch
	{
		public static bool IsEnabled()
		{
			return IsDebugBuild() || Environment.GetEnvironmentVariable("PANDORITY_DEBUG") != null;
		}

		private static bool IsDebugBuild()
		{
#if DEBUG
			return true;
#else
			return false;
#endif
		}
	}
}
