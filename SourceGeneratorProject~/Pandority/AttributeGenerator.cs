namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System;

	internal class AttributeGenerator
	{
		public void Generate(GeneratorExecutionContext context, string attributeName)
		{
			context.AddSource($"{attributeName}.g.cs", GetAttributeSource(attributeName));
		}

		public AttributeTargets AttributeTargets { get; set; } = AttributeTargets.Assembly;

		private string GetAttributeSource(string attributeName) =>
			@$"using System;

[AttributeUsage(AttributeTargets.{AttributeTargets})]
internal class {attributeName} : Attribute
{{
}}";
	}
}