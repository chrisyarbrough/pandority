namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class AccessorClass
	{
		public readonly ClassDeclarationSyntax ClassDeclaration;
		public readonly List<FieldInfo> Fields;

		public AccessorClass(ClassDeclarationSyntax classDeclaration, List<FieldInfo> fields)
		{
			ClassDeclaration = classDeclaration;
			Fields = fields;
		}

		public string Name => ClassDeclaration.Identifier.ToString();
	}

	internal class FieldInfo
	{
		public readonly FieldDeclarationSyntax Field;
		public readonly AccessorFlags Flags;

		public FieldInfo(FieldDeclarationSyntax field, AccessorFlags flags)
		{
			Field = field;
			Flags = flags;
		}
	}

	[Flags]
	internal enum AccessorFlags
	{
		Read = 1,
		Write = 2,
		ReadWrite = Read | Write
	}

	internal class AccessorsFinder : ISyntaxReceiver
	{
		public readonly List<AccessorClass> AccessorClasses = new();

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (IsExtendableClass(syntaxNode, out ClassDeclarationSyntax classDeclaration))
			{
				var fields = new List<FieldInfo>();

				foreach (FieldDeclarationSyntax field in classDeclaration.Members.OfType<FieldDeclarationSyntax>())
				{
					bool hasReadAttribute = HasReadAttribute(field);
					bool hasWriteAttribute = HasWriteAttribute(field);

					if (hasReadAttribute || hasWriteAttribute)
					{
						AccessorFlags flags = DetermineAccessorFlags(hasReadAttribute, hasWriteAttribute);
						fields.Add(new FieldInfo(field, flags));
					}
				}

				if (fields.Count > 0)
				{
					AccessorClasses.Add(new AccessorClass(classDeclaration, fields));
				}
			}
		}

		private static AccessorFlags DetermineAccessorFlags(bool hasRead, bool hasWrite)
		{
			if (hasRead && hasWrite)
			{
				return AccessorFlags.ReadWrite;
			}
			else if (hasRead)
			{
				return AccessorFlags.Read;
			}
			else if (hasWrite)
			{
				return AccessorFlags.Write;
			}
			else
			{
				throw new InvalidOperationException("Field has neither read nor write access.");
			}
		}

		private static bool HasReadAttribute(FieldDeclarationSyntax node)
		{
			return node.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.ToString() == "ReadAccess"));
		}

		private static bool HasWriteAttribute(FieldDeclarationSyntax node)
		{
			return node.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.ToString() == "WriteAccess"));
		}

		private static bool IsExtendableClass(SyntaxNode? node, out ClassDeclarationSyntax containingClass)
		{
			if (node is ClassDeclarationSyntax classDeclaration)
			{
				containingClass = classDeclaration;
				return classDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
			}

			containingClass = null!;
			return false;
		}
	}
}