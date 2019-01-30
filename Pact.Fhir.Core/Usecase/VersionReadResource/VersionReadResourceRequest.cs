namespace Pact.Fhir.Core.Usecase.VersionReadResource
{
  using Pact.Fhir.Core.Usecase.ReadResource;

  public class VersionReadResourceRequest : ReadResourceRequest
  {
    public string VersionId { get; set; }
  }
}