namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System.Linq;

	/// <summary>
	/// Generates the <c>PandorityTarget</c> attribute which marks a user assembly to be processed by Pandority.
	/// </summary>
	internal class TargetAttributeFilter : IAssemblyFilter
	{
		private const string attributeName = "PandorityTargetAttribute";

		private static string AttributeSource => @$"using System;

[AttributeUsage(AttributeTargets.Assembly)]
internal class {attributeName} : Attribute
{{
}}";

		public bool IsTargetAssembly(GeneratorExecutionContext context)
		{
			context.AddSource($"{attributeName}.generated.cs", AttributeSource);

			return context.Compilation.Assembly.GetAttributes()
				.Any(x => x.AttributeClass?.Name is "PandorityTarget" or "PandorityTargetAttribute");
		}
	}
}
