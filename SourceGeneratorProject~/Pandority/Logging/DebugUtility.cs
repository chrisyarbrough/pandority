namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;

	internal class DebugUtility
	{
		public static bool IsDebugBuild
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}

		private readonly ILog log;

		public DebugUtility(ILog log)
		{
			this.log = log;
		}

		/// <summary>
		/// This is intended to be used when manually invoking the compiler for debugging like so:
		/// <c>dotnet exec csc.dll @UserAssembly.rsp /define:PANDORITY_WAIT_FOR_DEBUGGER</c>
		/// </summary>
		[Conditional("DEBUG")]
		public void WaitForDebugger(GeneratorExecutionContext context)
		{
			if (context.ParseOptions != null &&
			    context.ParseOptions.PreprocessorSymbolNames.Contains("PANDORITY_WAIT_FOR_DEBUGGER"))
			{
				log.WriteLine("Waiting for debugger to attach...");
				while (!Debugger.IsAttached)
				{
					Thread.Sleep(250);
				}
				log.WriteLine("Debugger attached.");
			}
		}
	}
}