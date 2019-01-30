namespace Pact.Fhir.Core.Usecase.UpdateResource
{
  public class UpdateResourceRequest
  {
    public string ResourceId { get; set; }

    public string ResourceJson { get; set; }
  }
}