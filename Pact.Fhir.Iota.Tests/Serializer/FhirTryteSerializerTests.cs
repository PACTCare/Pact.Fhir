namespace Pact.Fhir.Core.Tests.Serialize
{
  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Iota.Serializer;

  /// <summary>
  /// The fhir tryte serializer tests.
  /// </summary>
  [TestClass]
  public class FhirTryteSerializerTests
  {
    /// <summary>
    /// The test json serializing.
    /// </summary>
    [TestMethod]
    public void TestJsonSerializing()
    {
      var serializer = new FhirJsonTryteSerializer();

      var patientAsTrytes = serializer.Serialize(FhirResourceProvider.Patient);
      var unserialized = serializer.Deserialize<Patient>(patientAsTrytes);

      Assert.IsTrue(FhirResourceProvider.Patient.IsExactly(unserialized));
    }
  }
}