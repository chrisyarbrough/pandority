namespace Pandority
{
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis.CSharp;

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
			if (syntaxNode is not EnumDeclarationSyntax enumDeclaration)
				return;

			if (IsVisible(enumDeclaration) == false)
				return;

			if (HasFlagsAttribute(enumDeclaration) == false)
				return;

			EnumDeclarations.Add(enumDeclaration);
		}

		/// <summary>
		/// Determines if the enum is visible to the client code.
		/// </summary>
		private static bool IsVisible(MemberDeclarationSyntax? syntaxNode)
		{
			while (syntaxNode != null)
			{
				if (syntaxNode.Modifiers.Any(IsHidden))
				{
					// It is not possible to generate an extension method for hidden enums.
					return false;
				}

				syntaxNode = syntaxNode.Parent as MemberDeclarationSyntax;
			}

			// All outer classes and the enum itself are internal or public.
			return true;
		}

		private static bool IsHidden(SyntaxToken token)
		{
			return token.IsKind(SyntaxKind.ProtectedKeyword) || token.IsKind(SyntaxKind.PrivateKeyword);
		}

		private static bool HasFlagsAttribute(EnumDeclarationSyntax enumDeclaration)
		{
			return enumDeclaration.AttributeLists.Any(HasFlagsAttribute);
		}

		private static bool HasFlagsAttribute(AttributeListSyntax attributeList)
		{
			return attributeList.Attributes.Any(IsFlagsAttribute);
		}

		private static bool IsFlagsAttribute(AttributeSyntax attribute)
		{
			string name = attribute.Name.ToString();
			return name is "System.Flags" or "System.FlagsAttribute" or "Flags" or "FlagsAttribute";
		}
	}
}
