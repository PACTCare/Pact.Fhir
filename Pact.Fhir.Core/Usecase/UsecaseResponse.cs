namespace Pact.Fhir.Core.Usecase
{
  using Hl7.Fhir.Model;

  public class UsecaseResponse
  {
    public Resource Resource { get; set; }

    public ResponseCode Code { get; set; }

    public string ExceptionMessage { get; set; }
  }
}