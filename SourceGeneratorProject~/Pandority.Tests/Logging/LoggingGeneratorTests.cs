namespace Pandority.Tests;

public class LoggingGeneratorTests
{
	[Fact]
	public void InitializeExceptionsAreLogged()
	{
		var crashLog = Substitute.For<ILog>();
		var debugLog = Substitute.For<ILog>();
		var generator = Substitute.For<LoggingGenerator>(crashLog, debugLog);

		var testException = new Exception("InitTest");

		generator.When(g => g.Initialize(Arg.Any<GeneratorInitializationContext>())).Do(_ => throw testException);

		generator.Initialize(new GeneratorInitializationContext());

		crashLog.Received().WriteLine(testException, null);
	}

	[Fact]
	public void ExecuteExceptionsAreLogged()
	{
		var crashLog = Substitute.For<ILog>();
		var debugLog = Substitute.For<ILog>();
		var generator = Substitute.For<LoggingGenerator>(crashLog, debugLog);

		var testException = new Exception("ExecTest");

		generator.When(g => g.Execute(Arg.Any<GeneratorExecutionContext>())).Do(_ => throw testException);

		var context = new GeneratorExecutionContext();
		generator.Execute(context);

		crashLog.Received().WriteLine(testException, context);
	}
}
