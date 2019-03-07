namespace Pact.Fhir.Core.Usecase.BatchProcess
{
  using Hl7.Fhir.Model;

  public class BatchProcessRequest
  {
    public Bundle Bundle { get; set; }
  }
}