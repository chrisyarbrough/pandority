namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Identifies the user editor classes that are partial and can receive generated code.
	/// </summary>
	internal class EditorFinder : ISyntaxReceiver
	{
		/// <summary>
		/// Keys are the target Object types, values are the Editor types.
		/// </summary>
		public readonly Dictionary<TypeSyntax, ClassDeclarationSyntax> CustomEditors = new();

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is ClassDeclarationSyntax classDeclaration &&
			    classDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)) &&
			    TryGetEditorAttribute(classDeclaration, out TypeOfExpressionSyntax typeofExpression))
			{
				TypeSyntax targetType = typeofExpression.Type;
				CustomEditors[targetType] = classDeclaration;
			}
		}

		private static bool TryGetEditorAttribute(ClassDeclarationSyntax classDeclaration,
			out TypeOfExpressionSyntax typeOfExpression)
		{
			AttributeSyntax? editorAttribute = classDeclaration.AttributeLists.SelectMany(list => list.Attributes)
				.FirstOrDefault(attribute => attribute.Name is IdentifierNameSyntax
				{
					Identifier: { ValueText: "CustomEditor" }
				});

			if (editorAttribute is { ArgumentList: { Arguments: var arguments } })
			{
				typeOfExpression = arguments.Select(x => x.Expression)
					.OfType<TypeOfExpressionSyntax>().FirstOrDefault()!;

				return true;
			}

			typeOfExpression = null!;
			return false;
		}
	}
}
