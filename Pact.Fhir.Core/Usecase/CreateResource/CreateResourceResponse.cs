namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using Hl7.Fhir.Model;

  public class CreateResourceResponse : UsecaseResponse
  {
    public DomainResource Resource { get; set; }
  }
}