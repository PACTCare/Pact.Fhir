namespace Pact.Fhir.Core.Tests.Serialize
{
  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Serializer;
  using Pact.Fhir.Core.Tests.Helpers;

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
      var patient = PatientHelpers.GetPatient();
      var serializer = new FhirJsonTryteSerializer();

      var patientAsTrytes = serializer.Serialize(patient);
      var unserialized = serializer.Deserialize<Patient>(patientAsTrytes);

      Assert.IsTrue(patient.IsExactly(unserialized));
    }

    /// <summary>
    /// The test xml serializing.
    /// </summary>
    [TestMethod]
    public void TestXmlSerializing()
    {
      var patient = PatientHelpers.GetPatient();
      var serializer = new FhirXmlTryteSerializer();

      var patientAsTrytes = serializer.Serialize(patient);
      var unserialized = serializer.Deserialize<Patient>(patientAsTrytes);

      Assert.IsTrue(patient.IsExactly(unserialized));
    }
  }
}