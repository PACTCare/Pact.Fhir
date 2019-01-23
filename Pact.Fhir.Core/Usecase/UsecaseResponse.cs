namespace Pact.Fhir.Core.Usecase
{
  using Hl7.Fhir.Model;

  public class UsecaseResponse
  {
    public ResponseCode Code { get; set; }

    public string ExceptionMessage { get; set; }

    public DomainResource Resource { get; set; }
  }
}