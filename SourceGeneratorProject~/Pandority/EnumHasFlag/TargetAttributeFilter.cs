namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System.Linq;

	/// <summary>
	/// Identifies user assemblies that should be processed by Pandority.
	/// </summary>
	/// <remarks>
	/// Users mark an assembly by adding the <c>PandorityTarget</c> attribute to it.
	/// </remarks>
	internal class TargetAttributeFilter : IAssemblyFilter
	{
		public bool IsTargetAssembly(Compilation compilation)
		{
			return compilation.Assembly.GetAttributes()
				.Any(x => x.AttributeClass?.Name is "PandorityTarget" or "PandorityTargetAttribute");
		}
	}
}