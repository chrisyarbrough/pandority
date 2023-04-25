namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System.Linq;

	public static class TargetAttribute
	{
		public static bool IsTargetAssembly(GeneratorExecutionContext context)
		{
			context.AddSource("PandorityTargetAttribute.generated.cs", AttributeSource);
			return HasAttributeApplied(context.Compilation.Assembly);
		}

		private static string AttributeSource => @"using System;

[AttributeUsage(AttributeTargets.Assembly)]
internal class PandorityTargetAttribute : Attribute
{
}";

		private static bool HasAttributeApplied(IAssemblySymbol assembly)
		{
			return assembly.GetAttributes()
				.Any(x => x.AttributeClass?.Name is "PandorityTarget" or "PandorityTargetAttribute");
		}
	}
}
