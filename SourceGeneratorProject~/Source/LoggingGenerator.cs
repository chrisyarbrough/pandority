namespace Pandority
{
	using System;
	using System.IO;
	using Microsoft.CodeAnalysis;

	/// <summary>
	/// A generator that logs any exception that occurs during the code generation to a file.
	/// </summary>
	[Generator]
	internal class LoggingGenerator : ISourceGenerator
	{
		private readonly ISourceGenerator generator;

		public LoggingGenerator()
		{
			this.generator = new EnumHasFlagGenerator();
		}

		public void Initialize(GeneratorInitializationContext context)
		{
			try
			{
				generator.Initialize(context);
			}
			catch (Exception ex)
			{
				File.WriteAllText("Pandority.Initialize.Crash.log", ex.ToString());
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
				File.WriteAllText("Pandority.Execute.Crash.log", ex.ToString());
			}
		}
	}
}
