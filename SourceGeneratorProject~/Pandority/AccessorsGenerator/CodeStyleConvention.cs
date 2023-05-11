namespace Pandority
{
	public static class CodeStyleConvention
	{
		/// <summary>
		/// Attempts to convert a field name to a valid property name, following the most common guidelines.
		/// <p/>
		/// Also see: <a href="https://docs.unity3d.com/ScriptReference/ObjectNames.NicifyVariableName.html">
		/// ObjectNames.NicifyVariableName</a>
		/// </summary>
		public static bool TryConvertToPropertyName(string fieldName, out string propertyName)
		{
			if (fieldName.StartsWith("m_"))
			{
				propertyName = ToPascalCase(fieldName[2..]);
				return true;
			}

			if (fieldName.StartsWith("_") && char.IsLower(fieldName[1]))
			{
				propertyName = ToPascalCase(fieldName[1..]);
				return true;
			}

			if ((fieldName[0] == 'k' && char.IsUpper(fieldName[1])) ||
			    fieldName.StartsWith("k_") ||
			    fieldName.StartsWith("c_"))
			{
				propertyName = fieldName;
				return false;
			}

			if (char.IsLower(fieldName[0]))
			{
				propertyName = ToPascalCase(fieldName);
				return true;
			}

			propertyName = fieldName;
			return false;
		}

		private static string ToPascalCase(string value)
		{
			return char.ToUpper(value[0]) + value[1..];
		}
	}
}