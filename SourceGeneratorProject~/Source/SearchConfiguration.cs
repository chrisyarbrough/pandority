using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xarbrough.Pandority
{
	internal static class SearchConfiguration
	{
		// TODO: Make this configurable by the user.
		private static readonly string[] namespaces = { "Com.InnoGames" };

		public static bool IsInUserScope(EnumDeclarationSyntax enumDeclaration)
		{
			// Avoid generating potentially hundreds of classes for builtin enums.
			var namespaceDeclaration = enumDeclaration.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

			if (namespaceDeclaration == null)
				return false;

			return namespaces.Any(x => namespaceDeclaration.Name.ToString().StartsWith(
				x, StringComparison.OrdinalIgnoreCase));
		}
	}
}
