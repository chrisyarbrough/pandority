using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Xarbrough.Pandority
{
	/// <summary>
	/// Identifies the user enums that the source generator should generate code for.
	/// </summary>
	internal class EnumFinder : ISyntaxReceiver
	{
		/// <summary>
		/// The enums that the generator will generate code for.
		/// </summary>
		public List<EnumDeclarationSyntax> EnumDeclarations { get; } = new(8);

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (IsFlagsEnum(syntaxNode, out EnumDeclarationSyntax enumDeclaration))
			{
				if (SearchConfiguration.IsInUserScope(enumDeclaration))
				{
					EnumDeclarations.Add(enumDeclaration);
				}
			}
		}

		private static bool IsFlagsEnum(SyntaxNode syntaxNode, out EnumDeclarationSyntax enumDeclaration)
		{
			if (syntaxNode is EnumDeclarationSyntax enumDeclarationSyntax)
			{
				enumDeclaration = enumDeclarationSyntax;

				// Ignore enum nested within another class for now, because it requires more handling.
				if (enumDeclaration.Parent is not NamespaceDeclarationSyntax)
					return false;

				return enumDeclarationSyntax.AttributeLists.Any(
					a => a.Attributes.Any(attribute => attribute.Name.ToString() is
						"System.Flags" or "System.FlagsAttribute" or "Flags" or "FlagsAttribute"));
			}
			else
			{
				enumDeclaration = null;
				return false;
			}
		}
	}
}
