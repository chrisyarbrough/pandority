namespace Pandority.Tests;

public class FileNameBuilderTests
{
	[Fact]
	public void FileNameContainsNoInvalidChars()
	{
		// This test is insufficient, but a reminder to pay attention to the character set.
		// If this test passes on macOS, it might still fail on Windows.
		var builder = new FileNameBuilder("Debug_Test");
		string fileName = builder.Build(null);
		IEnumerable<string> invalidChars = Path.GetInvalidFileNameChars().Select(x => x.ToString());
		fileName.Should().NotContainAny(invalidChars);
	}
}