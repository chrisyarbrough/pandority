namespace Pandority
{
	using System;
	using System.Text;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Text;

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
	/// <seealso cref="LoggingGenerator"/>
	internal class EnumHasFlagGenerator : ISourceGenerator
	{
		private readonly Log log = new($"{nameof(EnumHasFlagGenerator)}.log");

		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new EnumFinder());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not EnumFinder enumFinder)
				return;

			log.Debug($"Visiting assembly: {context.Compilation.AssemblyName}");

			if (!TargetAttribute.IsTargetAssembly(context))
				return;

			log.Debug("Found assembly target attribute. Generating enum extensions...");

			foreach (EnumDeclarationSyntax enumDeclaration in enumFinder.EnumDeclarations)
			{
				SemanticModel model = context.Compilation.GetSemanticModel(enumDeclaration.SyntaxTree);

				if (model.GetDeclaredSymbol(enumDeclaration) is INamedTypeSymbol enumSymbol)
				{
					SourceText sourceText = GenerateHasFlagExtension(enumSymbol);
					context.AddSource($"{enumSymbol.Name}PandorityExtensions.generated.cs", sourceText);
					log.Debug($"->{enumSymbol.Name}");
				}
			}
		}

		private SourceText GenerateHasFlagExtension(INamedTypeSymbol enumSymbol)
		{
			string namespaceName = enumSymbol.ContainingNamespace.ToDisplayString();
			string enumTypeName = enumSymbol.Name;
			string fullEnumTypeName = GetFullTypeName(enumSymbol);
			string visibility = GetVisibility(enumSymbol);

			string source = $@"namespace {namespaceName}
{{
	{visibility} static class {enumTypeName}PandorityExtensions
	{{
		public static bool HasFlagNonAlloc(this {fullEnumTypeName} value, {fullEnumTypeName} flag)
		{{
			return (value & flag) == flag;
		}}
	}}
}}
";
			// The explicit encoding is required.
			return SourceText.From(source, Encoding.UTF8);
		}

		private static string GetFullTypeName(INamedTypeSymbol enumSymbol)
		{
			// Handle enums nested within another class.
			if (enumSymbol.ContainingType != null)
				return GetFullTypeName(enumSymbol.ContainingType) + "." + enumSymbol.Name;

			return enumSymbol.Name;
		}

		/// <summary>
		/// Determines weather the extension method should be public or internal.
		/// </summary>
		private static string GetVisibility(INamedTypeSymbol typeSymbol)
		{
			while (typeSymbol.ContainingType != null)
			{
				// If any of the outer classes or the enum is internal, the extension method must follow suit.
				if (typeSymbol.DeclaredAccessibility == Accessibility.Internal)
					return "internal";

				typeSymbol = typeSymbol.ContainingType;
			}

			return typeSymbol.DeclaredAccessibility.ToString().ToLowerInvariant();
		}
	}
}
