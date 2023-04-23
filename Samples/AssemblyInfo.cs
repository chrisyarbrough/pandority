using System;

[assembly: SourceGeneratorNamespace("Com.InnoGames")]

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class SourceGeneratorNamespaceAttribute : Attribute
{
	public string Namespace { get; }

	public SourceGeneratorNamespaceAttribute(string ns)
	{
		Namespace = ns;
	}
}
