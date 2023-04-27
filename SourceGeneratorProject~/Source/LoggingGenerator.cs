namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System.IO;
	using System.Runtime.InteropServices;
	using System;
	using static System.Environment;

	/// <summary>
	/// A generator that logs any exception that occurs during the code generation to a file.
	/// </summary>
	[Generator]
	internal class LoggingGenerator : ISourceGenerator
	{
		private readonly ISourceGenerator generator;

		public LoggingGenerator()
		{
			generator = new EnumHasFlagGenerator();
		}

		public void Initialize(GeneratorInitializationContext context)
		{
			try
			{
				generator.Initialize(context);
			}
			catch (Exception ex)
			{
				LogException(ex);
			}
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				generator.Execute(context);
			}
			catch (Exception ex)
			{
				LogException(ex);
			}
		}

		private static void LogException(Exception ex)
		{
			if (!TryGetLogDirectory(out string logDirectory))
				return;

			Directory.CreateDirectory(logDirectory);

			string logFilePath = Path.Combine(logDirectory, "Crash.log");
			File.WriteAllText(logFilePath, ex.ToString());
		}

		/// <summary>
		/// Attempts to find a directory where log files can be written following the convention of the Unity log files.
		/// </summary>
		private static bool TryGetLogDirectory(out string logDirectory)
		{
			logDirectory = null;

			// This follows the convention of the Unity log files.
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				logDirectory = GetFolderPath(SpecialFolder.UserProfile) + "/Library/Logs/Pandority";

			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				logDirectory = GetFolderPath(SpecialFolder.LocalApplicationData) + "\\Pandority";

			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				logDirectory = GetFolderPath(SpecialFolder.UserProfile) + "/.config/Pandority";

			return logDirectory != null;
		}
	}
}
