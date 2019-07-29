namespace Pact.Fhir.Core.Usecase.ReadResourceVersion
{
  using Pact.Fhir.Core.Usecase.ReadResource;

  public class ReadResourceVersionRequest : ReadResourceRequest
  {
    public string VersionId { get; set; }
  }
}