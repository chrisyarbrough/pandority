namespace Pandority.Tests;

using System.Text.RegularExpressions;

public class SharedFileTests
{
	[Fact]
	public void ConcurrentWritesAreAtomic()
	{
		string filePath = Path.Combine(Path.GetTempPath(), "PandorityTest.log");
		var file = new SharedFile(filePath);

		try
		{
			// Ensure that both threads start writing at roughly the same time.
			var threadBarrier = new Barrier(2);
			const int writeCount = 50;

			void WriteToFile(string text)
			{
				threadBarrier.SignalAndWait();

				// Write multiple time to increase the chance of interleaving.
				for (int i = 0; i < writeCount; i++)
					file.WriteAtomicLine(text);
			}

			Task task1 = Task.Run(() => WriteToFile("A"));
			Task task2 = Task.Run(() => WriteToFile("B"));
			Task.WaitAll(task1, task2);


			// If both tasks wrote correctly the file should contain 50 A's and 50 B's on 100 lines.
			string fileContent = File.ReadAllText(filePath);
			Assert.Equal(writeCount, Regex.Matches(fileContent, "A").Count);
			Assert.Equal(writeCount, Regex.Matches(fileContent, "B").Count);
			Assert.Equal(writeCount + writeCount, File.ReadAllLines(filePath).Length);
		}
		finally
		{
			File.Delete(filePath);
		}
	}
}