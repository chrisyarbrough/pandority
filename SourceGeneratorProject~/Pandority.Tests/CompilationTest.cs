namespace Pandority.Tests;

/// <summary>
/// A test that requires a <see cref="Compilation"/> to be created.
/// </summary>
public abstract class CompilationTest
{
	/// <summary>
	/// Creates a simple fake <see cref="Compilation"/> from the given source text as input for testing.
	/// </summary>
	/// <remarks>
	/// The return value is minimalistic and only contains the necessary information for the tests.
	/// It is in no way a full replica, e.g. it doesn't contain metadata references, etc.
	/// </remarks>
	protected static Compilation CreateCompilation(string source)
	{
		return CSharpCompilation.Create("TestCompilation",
			new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp9)) });
	}
}
