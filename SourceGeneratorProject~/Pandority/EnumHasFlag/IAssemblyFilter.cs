namespace Pandority
{
	using Microsoft.CodeAnalysis;

	internal interface IAssemblyFilter
	{
		bool IsTargetAssembly(Compilation compilation);
	}
}