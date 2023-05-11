namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Text;
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Generates property getters and setters for fields marked with <c>ReadAccess</c> and <c>WriteAccess</c> attributes.
	/// </summary>
	[Generator]
	internal class AccessorsGenerator : LoggingGenerator
	{
		public AccessorsGenerator() : this(
			crashLog: new Log(new FileNameBuilder("Crash_AccessorGenerator")),
			debugLog: GetDebugLog("Debug_AccessorGenerator"))
		{
		}

		public AccessorsGenerator(ILog crashLog, ILog debugLog) : base(crashLog, debugLog)
		{
		}

		protected override void InitializeGenerator(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new AccessorsFinder());
		}

		protected override void ExecuteGenerator(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not AccessorsFinder finder)
				return;

			var attribute = new AttributeGenerator { AttributeTargets = AttributeTargets.Field };
			attribute.Generate(context, "ReadAccess");
			attribute.Generate(context, "WriteAccess");

			foreach (AccessorClass accessorClass in finder.AccessorClasses)
			{
				if (context.CancellationToken.IsCancellationRequested)
					break;

				Log.WriteLine("Found: " + accessorClass.Name);

				ClassDeclarationSyntax classDeclaration = accessorClass.ClassDeclaration;
				CompilationUnitSyntax compilationUnit = classDeclaration.SyntaxTree.GetCompilationUnitRoot();

				ClassDeclarationSyntax modifiedClassDeclaration = classDeclaration
					.WithMembers(GenerateMembers(accessorClass.Fields))
					.WithBaseList(null);

				SourceText sourceText = SyntaxFactory.CompilationUnit()
					.WithUsings(compilationUnit.Usings)
					.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(modifiedClassDeclaration))
					.NormalizeWhitespace(indentation: "\t", eol: "\n")
					.GetText(Encoding.UTF8);

				context.AddSource($"{accessorClass.Name}.g.cs", sourceText);
			}
		}

		private SyntaxList<MemberDeclarationSyntax> GenerateMembers(List<FieldInfo> fields)
		{
			SyntaxList<MemberDeclarationSyntax> list = SyntaxFactory.List<MemberDeclarationSyntax>();

			foreach (FieldInfo fieldInfo in fields)
			{
				string fieldName = fieldInfo.Field.Declaration.Variables[0].Identifier.Text;

				if (!CodeStyleConvention.TryConvertToPropertyName(fieldName, out string propertyName))
				{
					Log.WriteLine($"Failed to convert field name '{fieldName}' to property name.");
					continue;
				}

				PropertyDeclarationSyntax propertyDeclaration = SyntaxFactory.PropertyDeclaration(
					fieldInfo.Field.Declaration.Type, propertyName);

				SyntaxList<AccessorDeclarationSyntax> accessors = SyntaxFactory.List<AccessorDeclarationSyntax>();

				if (fieldInfo.Flags.HasFlag(AccessorFlags.Read))
				{
					AccessorDeclarationSyntax getAccessor = SyntaxFactory
						.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
						.WithExpressionBody(
							SyntaxFactory.ArrowExpressionClause(
								SyntaxFactory.IdentifierName(fieldName)))
						.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

					accessors = accessors.Add(getAccessor);
				}

				if (fieldInfo.Flags.HasFlag(AccessorFlags.Write))
				{
					AccessorDeclarationSyntax setAccessor = SyntaxFactory
						.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
						.WithExpressionBody(
							SyntaxFactory.ArrowExpressionClause(
								SyntaxFactory.AssignmentExpression(
									SyntaxKind.SimpleAssignmentExpression,
									SyntaxFactory.IdentifierName(fieldName),
									SyntaxFactory.IdentifierName("value"))))
						.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

					accessors = accessors.Add(setAccessor);
				}

				propertyDeclaration = propertyDeclaration
					.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
					.WithAccessorList(SyntaxFactory.AccessorList(accessors));

				list = list.Add(propertyDeclaration);
			}
			return list;
		}
	}
}