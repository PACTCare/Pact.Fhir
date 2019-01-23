namespace Pact.Fhir.Api.Cucumber.Definitions
{
  using System.Net;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using TechTalk.SpecFlow;

  [Binding]
  [Scope(Feature = "ReadResourceSuccess")]
  public class ReadResourceSteps : StepBase
  {
    [Then(@"I should be able to read his record")]
    public void ThenIShouldBeAbleToReadHisRecord()
    {
      var patientToRead = new FhirJsonParser().Parse<Patient>(this.LastResponseBody);
      this.CallApi($"api/fhir/Patient/{patientToRead.Id}", string.Empty, "GET");
      
      var patient = new FhirJsonParser().Parse<Patient>(this.LastResponseBody);

      Assert.AreEqual(HttpStatusCode.OK, this.LastResponseCode);
      Assert.IsTrue(this.LastResponseHeaders.Get("Content-Type").Contains("application/fhir+json"));
      Assert.AreEqual($"W/\"{patient.Meta.VersionId}\"", this.LastResponseHeaders.Get("ETag"));
      Assert.AreEqual(patient.Meta.LastUpdated.Value.ToString("O"), this.LastResponseHeaders.Get("Last-Modified"));
    }
  }
}