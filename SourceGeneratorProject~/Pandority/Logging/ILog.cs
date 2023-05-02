namespace Pandority
{
	using Microsoft.CodeAnalysis;

	internal interface ILog
	{
		/// <summary>
		/// A no-op implementation that is used to disable debug logging in release builds.
		/// </summary>
		public static readonly ILog Null = new NullLog();

		void WriteLine(object message, GeneratorExecutionContext? context = null);

		/// <remarks>
		/// Using <see cref="System.Diagnostics.ConditionalAttribute" /> is not possible for interface methods and
		/// the log class needs to be configurable for testing. The object-oriented approach also has more compile safety
		/// than sprinkling <c>#if DEBUG</c> statements throughout the code.
		/// </remarks>
		private class NullLog : ILog
		{
			public void WriteLine(object message, GeneratorExecutionContext? context = null)
			{
				// Do nothing.
			}
		}
	}
}