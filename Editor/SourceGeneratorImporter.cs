#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Pandority
{
	/// <summary>
	/// Ensures that source generator DLLs have the necessary settings to work in Unity.
	/// See: https://docs.unity3d.com/Manual/roslyn-analyzers.html
	/// </summary>
	internal class SourceGeneratorImporter : AssetPostprocessor
	{
		private void OnPreprocessAsset()
		{
			if (!assetImporter.importSettingsMissing || !IsSourceGeneratorPath(assetPath))
				return;

			var pluginImporter = assetImporter as PluginImporter;

			if (pluginImporter == null)
			{
				context.LogImportError($"Unexpected asset type {assetImporter.GetType()} for path:\n{assetPath}");
				return;
			}

			// Generators are not part of the regular compilation pipeline in Unity,
			// but treated specially by assigning an asset label.
			pluginImporter.SetCompatibleWithAnyPlatform(false);

			// Our tool is a source generator, but Unity uses the same label for both.
			EnsureLabel(assetImporter, "RoslynAnalyzer");

			pluginImporter.SaveAndReimport();
		}

		private static bool IsSourceGeneratorPath(string path)
		{
			return path.EndsWith("Pandority.dll", StringComparison.OrdinalIgnoreCase) ||
			       path.EndsWith("SourceGenerator.dll", StringComparison.OrdinalIgnoreCase);
		}

		private static void EnsureLabel(Object target, string label)
		{
			var labels = new List<string>(AssetDatabase.GetLabels(target));

			if (!labels.Contains(label))
			{
				labels.Add(label);
				AssetDatabase.SetLabels(target, labels.ToArray());
			}
		}
	}
}
#endif
