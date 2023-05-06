namespace Pandority
{
	using Microsoft.CodeAnalysis;

	internal interface IAssemblyFilter
	{
		bool IsTargetAssembly(GeneratorExecutionContext context);
	}
}
