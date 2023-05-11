namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System;

	/// <summary>
	/// A generator that logs any exception that occurs during the code generation to a file
	/// and supports waiting for a debugger to be attached during development.
	/// </summary>
	internal abstract class LoggingGenerator : ISourceGenerator
	{
		private readonly ILog crashLog;
		private readonly ILog debugLog;
		private readonly DebugUtility debugUtility;

		protected LoggingGenerator(ILog crashLog, ILog debugLog)
		{
			this.crashLog = crashLog;
			this.debugLog = debugLog;
			this.debugUtility = new DebugUtility(debugLog);
		}

		protected ILog Log => debugLog;

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
				debugUtility.WaitForDebugger(context);
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

		protected static ILog GetDebugLog(string nameHint)
		{
			if (DebugSwitch.IsEnabled())
				return new Log(new FileNameBuilder(nameHint));

			return ILog.Null;
		}
	}
}