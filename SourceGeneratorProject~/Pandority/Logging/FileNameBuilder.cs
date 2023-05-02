namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System.Reflection;

	internal class FileNameBuilder
	{
		private readonly string nameHint;

		/// <summary>
		/// Builds a file name following the template: {NameHint}_{CompilingAssembly}_{EntryAssembly}.log
		/// </summary>
		/// <remarks>
		/// The name includes the logs purpose and generator (e.g. Debug_MyGenerator).
		/// The compiling assembly is the user assembly that is currently being processed by the generator.
		/// The entry assembly is the context in which the generator is executed (e.g. Unity).
		/// </remarks>
		public FileNameBuilder(string nameHint)
		{
			this.nameHint = nameHint;
		}

		public string Build(GeneratorExecutionContext? context)
		{
			// Generators can execute in multiple processes (e.g., Unity and Rider) concurrently.
			// Moreover, within each process, multiple threads can process different assemblies in parallel.
			// Therefore, attempt to create a unique name to prevent overwriting log files.
			string entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name.Replace(".", "") ?? "Unknown";
			string compilingAssemblyName = context?.Compilation.AssemblyName ?? "Unknown";

			return $"{nameHint}_{compilingAssemblyName}_{entryAssemblyName}.log";
		}
	}
}