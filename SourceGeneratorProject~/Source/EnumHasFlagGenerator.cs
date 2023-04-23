using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Xarbrough.Pandority
{
	/// <summary>
	/// Uses the <see cref="EnumFinder"/> to find user defined enums that have a <see cref="System.FlagsAttribute"/>
	/// and generates a custom HasFlag extension method for them. This provides a way to avoid boxing the enum values.
	/// </summary>
	/// <remarks>
	/// Using the builtin <see cref="System.Enum.HasFlag"/> method causes boxing allocations because the underlying
	/// enum type is not known at compile time. This can be a problem in hot code paths in Unity. Implementing
	/// a generic HasFlag method is not so easy due to missing constraint support for this special case. Therefore,
	/// the most practical solution is to implement an extension method for each concrete enum type.
	/// </remarks>
	[Generator]
	internal class EnumHasFlagGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new EnumFinder());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not EnumFinder enumFinder)
				return;

			foreach (EnumDeclarationSyntax enumDeclaration in enumFinder.EnumDeclarations)
			{
				SemanticModel model = context.Compilation.GetSemanticModel(enumDeclaration.SyntaxTree);

				if (model.GetDeclaredSymbol(enumDeclaration) is not INamedTypeSymbol enumSymbol)
				{
					// This could happen if the user enum fails to compile.
					continue;
				}

				SourceText sourceText = GenerateHasFlagExtension(enumSymbol);
				context.AddSource($"{enumSymbol.Name}_HasFlagExtension.generated.cs", sourceText);
			}
		}

		private SourceText GenerateHasFlagExtension(INamedTypeSymbol enumSymbol)
		{
			string namespaceName = enumSymbol.ContainingNamespace.ToDisplayString();
			string enumTypeName = enumSymbol.Name;
			string visibility = enumSymbol.DeclaredAccessibility.ToString().ToLowerInvariant();

			return SourceText.From($@"
namespace {namespaceName}
{{
	{visibility} static class {enumTypeName}_HasFlagExtension
	{{
		public static bool HasFlagNonAlloc(this {enumTypeName} value, {enumTypeName} flag)
		{{
			return (value & flag) == flag;
		}}
	}}
}}
", Encoding.UTF8);
		}
	}
}
