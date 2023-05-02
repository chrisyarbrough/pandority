namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System;

	/// <summary>
	/// A generator that logs any exception that occurs during the code generation to a file.
	/// </summary>
	internal abstract class LoggingGenerator : ISourceGenerator
	{
		private readonly ILog crashLog;
		private readonly ILog debugLog;

		protected LoggingGenerator(ILog crashLog, ILog debugLog)
		{
			this.crashLog = crashLog;
			this.debugLog = debugLog;
		}

		protected ILog DebugLog => debugLog;

		public void Initialize(GeneratorInitializationContext context)
		{
			try
			{
				InitializeGenerator(context);
			}
			catch (Exception ex)
			{
				crashLog.WriteLine(ex);
			}
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				ExecuteGenerator(context);
			}
			catch (Exception ex)
			{
				crashLog.WriteLine(ex, context);
			}
		}

		protected virtual void InitializeGenerator(GeneratorInitializationContext context)
		{
		}

		protected abstract void ExecuteGenerator(GeneratorExecutionContext context);
	}
}