namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using System.Linq;

	/// <summary>
	/// A filter which returns true if any of its constituent filters return true.
	/// </summary>
	internal sealed class AnyOfFilter : IAssemblyFilter
	{
		private readonly IAssemblyFilter[] filters;

		public AnyOfFilter(params IAssemblyFilter[] filters)
		{
			this.filters = filters;
		}

		public bool IsTargetAssembly(GeneratorExecutionContext context)
		{
			return filters.Any(filter => filter.IsTargetAssembly(context));
		}
	}
}
