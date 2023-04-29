namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System;

	/// <summary>
	/// A generator that logs any exception that occurs during the code generation to a file.
	/// </summary>
	[Generator]
	internal class LoggingGenerator : ISourceGenerator
	{
		private readonly ISourceGenerator generator;
		private readonly Log log;

		public LoggingGenerator()
		{
			generator = new EnumHasFlagGenerator();
			log = new Log("Crash.log");
		}

		public void Initialize(GeneratorInitializationContext context)
		{
			try
			{
				generator.Initialize(context);
			}
			catch (Exception ex)
			{
				log.Exception(ex);
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
				log.Exception(ex);
			}
		}
	}
}
