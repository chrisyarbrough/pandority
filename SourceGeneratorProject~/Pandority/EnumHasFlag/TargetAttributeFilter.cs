namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System.Linq;

	/// <summary>
	/// Generates the <c>PandorityTarget</c> attribute which marks a user assembly to be processed by Pandority.
	/// </summary>
	internal class TargetAttributeFilter : IAssemblyFilter
	{
		private readonly string attributeName;

		public TargetAttributeFilter(string attributeName)
		{
			this.attributeName = attributeName;
		}

		public bool IsTargetAssembly(GeneratorExecutionContext context)
		{
			return context.Compilation.Assembly.GetAttributes().Any(x => x.AttributeClass?.Name == attributeName);
		}
	}
}