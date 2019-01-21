namespace Pact.Fhir.Api.Cucumber.Definitions
{
  using System.Linq;
  using System.Net;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Tests.Utils;

  using TechTalk.SpecFlow;

  using FeatureContext = Pact.Fhir.Api.Cucumber.Context.FeatureContext;

  [Binding]
  public class CreateResourceSteps : FeatureContext
  {
    private Patient Resource { get; set; }

    [Given(@"I have the patient ""(.*)""")]
    public void GivenIHaveThePatient(string patientName)
    {
      this.Resource = FhirResourceProvider.Patient;
      this.Resource.Name.First().Given = new[] { patientName };
    }

    [Then(@"I should see a valid patient JSON representation")]
    public void ThenIShouldSeeAValidPatientJsonRepresentation()
    {
      Assert.AreEqual(HttpStatusCode.Created, this.LastResponseCode);
      Assert.IsTrue(this.LastResponseHeaders.Get("ETag") != null);
      Assert.IsTrue(this.LastResponseHeaders.Get("Last-Modified") != null);
      Assert.IsTrue(this.IsValidJson(this.LastResponseBody));
    }

    [When(@"I create his FHIR record with Prefer ""(.*)""")]
    public void WhenICreateHisFhirRecordWithPrefer(string prefer)
    {
      this.CallApi(
        "api/fhir/create/Patient",
        new FhirJsonSerializer().SerializeToString(this.Resource),
        "POST",
        new WebHeaderCollection { { "Prefer", prefer } });
    }
  }
}