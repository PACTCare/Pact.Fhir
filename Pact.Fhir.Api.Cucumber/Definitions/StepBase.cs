namespace Pact.Fhir.Api.Cucumber.Definitions
{
  using System.Linq;
  using System.Net;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Tests.Utils;

  using TechTalk.SpecFlow;

  using FeatureContext = Pact.Fhir.Api.Cucumber.Context.FeatureContext;

  public class StepBase : FeatureContext
  {
    protected string Prefer { get; set; }

    protected Patient Resource { get; set; }

    [Given(@"I have the patient ""(.*)""")]
    public void GivenIHaveThePatient(string patientName)
    {
      this.Resource = FhirResourceProvider.Patient;
      this.Resource.Name.First().Given = new[] { patientName };
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
  }
}