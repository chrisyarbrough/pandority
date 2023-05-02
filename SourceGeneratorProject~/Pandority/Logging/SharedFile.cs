namespace Pandority
{
	using System.IO;
	using System.Threading;

	/// <summary>
	/// Ensures atomic access to a shared log file.
	/// </summary>
	/// <remarks>
	/// When two processes write log lines to a shared file, this class ensures that the lines are not
	/// interleaved or disrupted by the other process.
	/// </remarks>
	internal class SharedFile
	{
		private readonly bool isMutexOwner;
		private readonly Mutex mutex;
		private readonly StreamWriter writer;

		public SharedFile(string filePath)
		{
			writer = File.CreateText(filePath);
			writer.AutoFlush = true;

			string mutexName = Path.GetFileNameWithoutExtension(filePath);
			mutex = new Mutex(false, mutexName, out isMutexOwner);
		}

		~SharedFile()
		{
			// The mutex needs to be disposed because other generator processes may be waiting on it.
			// The writer however, will automatically be disposed when the finalizer runs.
			if (isMutexOwner)
				mutex.Dispose();

			if (writer != null)
				writer.Dispose();
		}

		public void WriteAtomicLine(object value)
		{
			bool isCurrentOwner = false;
			try
			{
				isCurrentOwner = mutex.WaitOne(10_000);
				writer.WriteLine(value);
			}
			catch (AbandonedMutexException)
			{
				// Inherit the shared mutex if another process has abandoned it (e.g. due to a crash).
				isCurrentOwner = true;
			}
			finally
			{
				if (isCurrentOwner)
					mutex.ReleaseMutex();
			}
		}
	}
}