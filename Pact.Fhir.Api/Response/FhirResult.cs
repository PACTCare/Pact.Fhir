namespace Pact.Fhir.Api.Response
{
  using System.Xml;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Rest;
  using Hl7.Fhir.Serialization;
  using Hl7.Fhir.Utility;

  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Formatters;
  using Microsoft.CodeAnalysis.CSharp.Syntax;

  using Task = System.Threading.Tasks.Task;

  public class FhirResult : IActionResult
  {
    public FhirResult(Resource resource, string contentType, SummaryType summaryType = SummaryType.False)
    {
      this.Resource = resource;
      this.ContentType = contentType;
      this.SummaryType = summaryType;
    }

    private string ContentType { get; }

    private Resource Resource { get; }

    private SummaryType SummaryType { get; }

    public void ExecuteResult(ActionContext context)
    {
      var result = this.GetResult();
      result.ExecuteResult(context);
    }

    /// <inheritdoc />
    public async Task ExecuteResultAsync(ActionContext context)
    {
      var result = this.GetResult();
      await result.ExecuteResultAsync(context);
    }

    private ObjectResult GetResult()
    {
      ObjectResult result;
      if (this.ContentType == FhirContentType.Xml)
      {
        result = new ObjectResult(new FhirXmlSerializer().SerializeToString(this.Resource, this.SummaryType))
                   {
                     ContentTypes = new MediaTypeCollection { FhirContentType.Xml }
                   };
      }
      else
        result = new ObjectResult(new FhirJsonSerializer().SerializeToDocument(this.Resource, this.SummaryType))
                   {
                     ContentTypes = new MediaTypeCollection { FhirContentType.Json }
                   };

      return result;
    }
  }
}