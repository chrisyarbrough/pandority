namespace Pandority
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.Text;
	using System.IO;
	using System.Linq;

	/// <summary>
	/// With this filter users can specify which assemblies should be processed by Pandority by adding a file named
	/// <c>Config.Pandority.additionalfile</c> in Unity's Assets folder. The file contains the names of the desired
	/// assemblies line by line.
	/// </summary>
	/// <remarks>
	/// https://docs.unity3d.com/ScriptReference/Compilation.ScriptCompilerOptions.RoslynAdditionalFilePaths.html
	/// </remarks>
	internal class ConfigFileFilter : IAssemblyFilter
	{
		public bool IsTargetAssembly(GeneratorExecutionContext context)
		{
			AdditionalText? configFile = context.AdditionalFiles.FirstOrDefault(
				file => Path.GetFileNameWithoutExtension(file.Path) == "Config.Pandority");

			if (configFile != null)
			{
				SourceText? fileContent = configFile.GetText(context.CancellationToken);
				if (fileContent != null)
				{
					return fileContent.Lines.Any(
						textLine => textLine.ToString().Trim() == context.Compilation.AssemblyName);
				}
			}

			return false;
		}
	}
}
