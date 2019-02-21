namespace Pact.Fhir.Core.Usecase.PatchResource
{
  public class PatchResourceRequest
  {
    public string Payload { get; set; }

    public string ResourceId { get; set; }
  }
}