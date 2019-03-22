namespace Pact.Fhir.Core.Usecase
{
  using Hl7.Fhir.Model;

  public class ResourceResponse : UsecaseResponse
  {
    public Resource Resource { get; set; }
  }
}