namespace Pact.Fhir.Api.Response
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Rest;
  using Hl7.Fhir.Serialization;

  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Formatters;

  using Task = System.Threading.Tasks.Task;

  public class JsonFhirResult : IActionResult
  {
    public JsonFhirResult(Resource resource, SummaryType summaryType = SummaryType.False)
    {
      this.Resource = resource;
      this.SummaryType = summaryType;
      this.Serializer = new FhirJsonSerializer();
    }

    private Resource Resource { get; }

    private FhirJsonSerializer Serializer { get; }

    private SummaryType SummaryType { get; }

    public void ExecuteResult(ActionContext context)
    {
      var result = new ObjectResult(this.Serializer.SerializeToDocument(this.Resource, this.SummaryType))
                     {
                       ContentTypes = new MediaTypeCollection { "application/fhir+json" }
                     };

      result.ExecuteResult(context);
    }

    /// <inheritdoc />
    public async Task ExecuteResultAsync(ActionContext context)
    {
      var result = new ObjectResult(this.Serializer.SerializeToDocument(this.Resource, this.SummaryType))
                     {
                       ContentTypes = new MediaTypeCollection { "application/fhir+json" }
                     };

      await result.ExecuteResultAsync(context);
    }
  }
}