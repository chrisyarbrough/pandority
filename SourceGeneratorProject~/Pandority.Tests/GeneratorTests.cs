namespace Pandority.Tests;

using System.Reflection;

public class GeneratorTests
{
	[Fact]
	public void AllGenerators_ShouldHaveParameterlessConstructor()
	{
		IEnumerable<Type> generatorTypes = typeof(ILog).Assembly.GetTypes()
			.Where(t => t.GetCustomAttribute<GeneratorAttribute>() != null);

		generatorTypes.Should().OnlyContain(t => t.GetConstructor(Type.EmptyTypes) != null,
			"Roslyn requires each generator to have a parameterless constructor");
	}
}
