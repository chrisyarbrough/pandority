namespace Pandority.Tests.EditorProperties;

public class EditorFinderTests : CompilationTest
{
	[Fact]
	public void FindsRegularPartialClass()
	{
		Compilation compilation = CreateCompilation(@"using UnityEditor;
using UnityEngine;

public class MyTarget : MonoBehaviour
{
}

[CustomEditor(typeof(MyTarget))]
public partial class MyTargetEditor : Editor
{
}");

		EditorFinder editorFinder = FakeSyntaxReceiverVisit(compilation);

		editorFinder.TargetToEditor.Count.Should().Be(1);
		(TypeSyntax targetType, ClassDeclarationSyntax editorClass) = editorFinder.TargetToEditor.First();

		targetType.Should().NotBeNull();
		editorClass.Should().NotBeNull();

		targetType.ToString().Should().Be("MyTarget");
		editorClass.Identifier.ToString().Should().Be("MyTargetEditor");
	}

	[Fact]
	public void IgnoresNonPartialClass()
	{
		Compilation compilation = CreateCompilation(@"using UnityEditor;
using UnityEngine;

public class MyTarget : MonoBehaviour
{
}

[CustomEditor(typeof(MyTarget))]
public class MyTargetEditor : Editor
{
}");

		EditorFinder editorFinder = FakeSyntaxReceiverVisit(compilation);
		editorFinder.TargetToEditor.Should().HaveCount(0);
	}

	[Fact]
	public void IgnoresEditorClassWithoutAttribute()
	{
		Compilation compilation = CreateCompilation(@"using UnityEditor;

public partial class MyTargetEditor : Editor
{
}");

		EditorFinder editorFinder = FakeSyntaxReceiverVisit(compilation);
		editorFinder.TargetToEditor.Should().HaveCount(0);
	}

	private static EditorFinder FakeSyntaxReceiverVisit(Compilation compilation)
	{
		// The unit under test is this ISyntaxReceiver class.
		var editorFinder = new EditorFinder();

		// Fake how the receiver would be called by the generator driver.
		foreach (SyntaxNode node in compilation.SyntaxTrees
			         .SelectMany(x => x.GetCompilationUnitRoot().DescendantNodes()))
		{
			editorFinder.OnVisitSyntaxNode(node);
		}

		return editorFinder;
	}
}