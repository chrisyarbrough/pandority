namespace Pandority.Tests.DataMemberGeneratorTests;

public class AccessorsGeneratorTests : CompilationTest
{
	[Fact]
	public void AttributesAreGenerated()
	{
		Compilation compilation = CreateCompilation("");

		var driver = CSharpGeneratorDriver.Create(new AccessorsGenerator());
		GeneratorDriverRunResult result = driver.RunGenerators(compilation).GetRunResult();

		result.GeneratedTrees.Should().HaveCount(2);

		bool hasReadAccessAttributeClass = ContainsAttributeClass(result, "ReadAccess");
		bool hasWriteAccessAttributeClass = ContainsAttributeClass(result, "WriteAccess");

		Assert.True(hasReadAccessAttributeClass, "'ReadAccess' attribute was not found in the generated code.");
		Assert.True(hasWriteAccessAttributeClass, "'WriteAccess' attribute was not found in the generated code.");
	}

	private static bool ContainsAttributeClass(GeneratorDriverRunResult result, string name)
	{
		return result.GeneratedTrees.Any(tree =>
		{
			return tree.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
				.Any(classDeclaration => classDeclaration.Identifier.Text == name);
		});
	}

	[Fact]
	public void PropertiesAreGenerated()
	{
		Compilation compilation = CreateCompilation(@"using UnityEngine;

public partial class MyDataObject : ScriptableObject
{
	[WriteAccess]
	[SerializeField]
	private int number;

	[ReadAccess]
	[SerializeField]
	private float volume;

	[ReadAccess, WriteAccess]
	[SerializeField]
	private string[] names;
}");

		var driver = CSharpGeneratorDriver.Create(new AccessorsGenerator());
		GeneratorDriverRunResult result = driver.RunGenerators(compilation).GetRunResult();

		result.GeneratedTrees.Should().HaveCountGreaterOrEqualTo(1);

		result.GeneratedTrees
			.SelectMany(x => x.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
			.Should().Contain(x => x.Identifier.ToString() == "MyDataObject");

		ClassDeclarationSyntax generatedClass = result.GeneratedTrees
			.SelectMany(x => x.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
			.First(x => x.Identifier.ToString() == "MyDataObject");

		generatedClass.DescendantNodes().OfType<PropertyDeclarationSyntax>().Should().SatisfyRespectively(
			one => ShouldHaveSyntaxKind(one, SyntaxKind.SetAccessorDeclaration),
			two => ShouldHaveSyntaxKind(two, SyntaxKind.GetAccessorDeclaration),
			three =>
			{
				ShouldHaveSyntaxKind(three, SyntaxKind.SetAccessorDeclaration);
				ShouldHaveSyntaxKind(three, SyntaxKind.GetAccessorDeclaration);
			});
	}

	private static void ShouldHaveSyntaxKind(PropertyDeclarationSyntax property, SyntaxKind syntaxKind)
	{
		property.AccessorList!.Accessors.Should().Contain(x => x.Kind() == syntaxKind);
	}
}