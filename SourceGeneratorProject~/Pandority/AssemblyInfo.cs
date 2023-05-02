using System.Runtime.CompilerServices;

[assembly:
	InternalsVisibleTo("Pandority.Tests"),
	// Expose types to the Castle DynamicProxy assembly which is used by NSubstitute.
	InternalsVisibleTo("DynamicProxyGenAssembly2"),
]
