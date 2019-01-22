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
    private string Prefer { get; set; }

    private Patient Resource { get; set; }

    [Given(@"I have the patient ""(.*)""")]
    public void GivenIHaveThePatient(string patientName)
    {
      this.Resource = FhirResourceProvider.Patient;
      this.Resource.Name.First().Given = new[] { patientName };
    }

    [Then(@"I should see a valid response")]
    public void ThenIShouldSeeAValidResponse()
    {
      switch (this.Prefer)
      {
        case "representation":
          var patient = new FhirJsonParser().Parse<Patient>(this.LastResponseBody);
          Assert.AreEqual(this.Resource.Name.First().Given.First(), patient.Name.First().Given.First());
          break;
        case "OperationOutcome":
          var outcome = new FhirJsonParser().Parse<OperationOutcome>(this.LastResponseBody);
          Assert.AreEqual("All OK", outcome.Issue.First().Details.Text);
          break;
      }

      this.AssertMetadata();
    }

    [When(@"I create his FHIR record with Prefer ""(.*)""")]
    public void WhenICreateHisFhirRecordWithPrefer(string prefer)
    {
      this.CallApi(
        "api/fhir/create/Patient",
        new FhirJsonSerializer().SerializeToString(this.Resource),
        "POST",
        new WebHeaderCollection { { "Prefer", prefer } });

      this.Prefer = prefer;
    }

    private void AssertMetadata()
    {
      Assert.AreEqual(HttpStatusCode.Created, this.LastResponseCode);

      Assert.IsTrue(this.LastResponseHeaders.Get("Content-Type").Contains("application/fhir+json"));
      Assert.IsTrue(this.LastResponseHeaders.Get("ETag") != null);
      Assert.IsTrue(this.LastResponseHeaders.Get("Last-Modified") != null);
      Assert.IsTrue(this.LastResponseHeaders.Get("Location") != null);
    }
  }
}