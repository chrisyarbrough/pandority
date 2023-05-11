namespace Pandority.Tests;

public class CodeStyleConventionTests
{
	[Theory]
	[InlineData("number")]
	[InlineData("_number")]
	[InlineData("m_number")]
	[InlineData("m_Number")]
	public void ConvertsFieldNameToPropertyName(string fieldName)
	{
		bool success = CodeStyleConvention.TryConvertToPropertyName(fieldName, out string propertyName);
		success.Should().BeTrue();
		propertyName.Should().Be("Number");
	}

	[Theory]
	[InlineData("a")]
	[InlineData("m_A")]
	[InlineData("m_a")]
	[InlineData("_a")]
	public void ConvertsShortFieldNameToPropertyName(string fieldName)
	{
		bool success = CodeStyleConvention.TryConvertToPropertyName(fieldName, out string propertyName);
		success.Should().BeTrue();
		propertyName.Should().Be("A");
	}

	[Fact]
	public void ConstantSpecialCaseIsHandled()
	{
		bool success = CodeStyleConvention.TryConvertToPropertyName("kilogramsPerMile", out string propertyName);
		success.Should().BeTrue();
		propertyName.Should().Be("KilogramsPerMile");
	}

	[Theory]
	[InlineData("Number")]
	[InlineData("_Number")]
	[InlineData("kNumber")]
	[InlineData("k_Number")]
	[InlineData("c_Number")]
	public void ReturnsFalseForInvalidFieldName(string fieldName)
	{
		// These field names cannot be converted to a property because we cannot assume know the convention.
		bool success = CodeStyleConvention.TryConvertToPropertyName(fieldName, out string name);
		success.Should().BeFalse($"because property name {name} looks bad");
	}
}