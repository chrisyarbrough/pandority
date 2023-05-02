namespace Pandority
{
	using Microsoft.CodeAnalysis;

	/// <summary>
	/// Generates the attribute that marks a user assembly to be processed by Pandority.
	/// </summary>
	internal static class TargetAttribute
	{
		private const string AttributeName = "PandorityTargetAttribute";

		private static string AttributeSource => @$"using System;

[AttributeUsage(AttributeTargets.Assembly)]
internal class {AttributeName} : Attribute
{{
}}";

		public static void GenerateAttribute(GeneratorExecutionContext context)
		{
			context.AddSource($"{AttributeName}.generated.cs", AttributeSource);
		}
	}
}
