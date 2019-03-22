namespace Pact.Fhir.Core.Tests.Services
{
  using System;

  using Hl7.Fhir.Rest;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Services;

  [TestClass]
  public class SummaryTypeParserTest
  {
    [DataTestMethod]
    [DataRow("false", "False")]
    [DataRow("", "False")]
    [DataRow(null, "False")]
    [DataRow("true", "True")]
    [DataRow("count", "Count")]
    [DataRow("text", "Text")]
    [DataRow("data", "Data")]
    public void TestSummaryTypeMap(string summaryType, string expectedEnumType)
    {
      var expectedType = Enum.Parse(typeof(SummaryType), expectedEnumType);
      var actualType = SummaryTypeParser.Parse(summaryType);

      Assert.AreEqual(expectedType, actualType);
    }
  }
}