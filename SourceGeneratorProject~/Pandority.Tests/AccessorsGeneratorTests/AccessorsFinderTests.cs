namespace Pandority.Tests.DataMemberGeneratorTests;

public class AccessorsFinderTests : CompilationTest
{
	[Fact]
	public void FindsRegularPartialClass()
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

		AccessorsFinder editorFinder = FakeSyntaxReceiverVisit(compilation);

		editorFinder.AccessorClasses.Should().HaveCount(1);

		AccessorClass info = editorFinder.AccessorClasses.First();
		info.ClassDeclaration.Identifier.ToString().Should().Be("MyDataObject");

		info.Fields.Should().HaveCount(3);
		info.Fields.Should().AllSatisfy(y =>
		{
			y.Should().NotBeNull();
			y.Field.Should().NotBeNull();
		});
		info.Fields.Should().SatisfyRespectively(
			first => first.Flags.Should().Be(AccessorFlags.Write),
			second => second.Flags.Should().Be(AccessorFlags.Read),
			third => third.Flags.Should().Be(AccessorFlags.ReadWrite)
		);
	}

	[Fact]
	public void IgnoresNonPartialClass()
	{
		Compilation compilation = CreateCompilation(@"using UnityEngine;

public class MyDataObject : ScriptableObject
{
	[ReadAccess]
	[SerializeField]
	private int number;

	[WriteAccess]
	[SerializeField]
	private string[] names;
}");

		AccessorsFinder editorFinder = FakeSyntaxReceiverVisit(compilation);
		editorFinder.AccessorClasses.Should().BeEmpty();
	}

	private static AccessorsFinder FakeSyntaxReceiverVisit(Compilation compilation)
	{
		// The unit under test is this ISyntaxReceiver class.
		var editorFinder = new AccessorsFinder();

		// Fake how the receiver would be called by the generator driver.
		foreach (SyntaxNode node in compilation.SyntaxTrees
			         .SelectMany(x => x.GetCompilationUnitRoot().DescendantNodes()))
		{
			editorFinder.OnVisitSyntaxNode(node);
		}

		return editorFinder;
	}
}