namespace Pact.Fhir.Core.Usecase.ReadResourceHistory
{
  public class ReadResourceHistoryRequest
  {
    public string ResourceId { get; set; }

    public string ResourceType { get; set; }
  }
}