namespace Pandority.Tests;

public class EnumHasFlagGeneratorTests : CompilationTest
{
	[Fact]
	public void TargetAttributeIsGenerated()
	{
		Compilation compilation = CreateCompilation(string.Empty);

		var driver = CSharpGeneratorDriver.Create(new EnumHasFlagGenerator());
		GeneratorDriverRunResult result = driver.RunGenerators(compilation).GetRunResult();

		bool hasTargetAttributeClass = result.GeneratedTrees.Any(tree =>
		{
			return tree.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
				.Any(classDeclaration => classDeclaration.Identifier.Text == "PandorityTargetAttribute");
		});

		Assert.True(hasTargetAttributeClass,
			"PandorityTargetAttribute was not found in the generated code.\n" +
			"It is required to mark user assemblies that should be searched for enums.");
	}

	[Fact]
	public void AssemblyFilterIsUsed()
	{
		Compilation compilation = CreateCompilation(string.Empty);

		var assemblyFilterMock = Substitute.For<IAssemblyFilter>();

		var driver = CSharpGeneratorDriver.Create(new EnumHasFlagGenerator(assemblyFilterMock, ILog.Null, ILog.Null));
		driver.RunGenerators(compilation).GetRunResult();

		assemblyFilterMock.Received().IsTargetAssembly(Arg.Any<GeneratorExecutionContext>());
	}
}
