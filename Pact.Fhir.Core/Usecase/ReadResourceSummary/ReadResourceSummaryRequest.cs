namespace Pact.Fhir.Core.Usecase.ReadResourceSummary
{
  public class ReadResourceSummaryRequest
  {
    public string ResourceId { get; set; }

    public string ResourceType { get; set; }

    public string SummaryType { get; set; }
  }
}