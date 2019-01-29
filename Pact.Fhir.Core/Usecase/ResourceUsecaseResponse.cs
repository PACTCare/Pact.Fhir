namespace Pact.Fhir.Core.Usecase
{
  using Hl7.Fhir.Model;

  public class ResourceUsecaseResponse : BaseResponse
  {
    public DomainResource Resource { get; set; }
  }
}