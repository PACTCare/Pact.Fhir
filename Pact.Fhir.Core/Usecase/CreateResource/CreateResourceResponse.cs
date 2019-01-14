namespace Pact.Fhir.Core.Usecase.CreateResource
{
  public class CreateResourceResponse : UsecaseResponse
  {
    public string LogicalId { get; set; }

    public string VersionId { get; set; }
  }
}