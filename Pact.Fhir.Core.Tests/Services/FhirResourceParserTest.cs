namespace Pact.Fhir.Core.Tests.Services
{
  using System;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Services;
  using Pact.Fhir.Core.Tests.Utils;

  [TestClass]
  public class FhirResourceParserTest
  {
    [TestMethod]
    [ExpectedException(typeof(UnsupportedResourceException))]
    public void TestResourceTypeIsNotSupportedShouldThrowException()
    {
      var parser = new FhirResourceParser(new FhirJsonParser());
      parser.Parse("UnknownType", string.Empty);
    }

    [DataRow("Patient", typeof(Patient))]
    [DataTestMethod]
    public void TestSupportedResourceShouldBeConvertible(string resourceType, Type expectedType)
    {
      var resource = typeof(FhirResourceProvider).GetProperty(resourceType).GetValue(null, null);
      var resourceJson = new FhirJsonSerializer().SerializeToString((DomainResource)resource);

      var parser = new FhirResourceParser(new FhirJsonParser());
      var parsedResource = parser.Parse(resourceType, resourceJson);

      Assert.IsInstanceOfType(parsedResource, expectedType);
    }
  }
}