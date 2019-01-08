namespace Pact.Fhir.Core.Usecase.ReadResource
{
  using Hl7.Fhir.Model;

  public class ReadResourceResponse : UsecaseResponse
  {
    public DomainResource Resource { get; set; }
  }
}