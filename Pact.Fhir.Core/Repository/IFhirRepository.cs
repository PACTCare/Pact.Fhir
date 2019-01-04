namespace Pact.Fhir.Core.Repository
{
  using Hl7.Fhir.Model;

  public interface IFhirRepository
  {
    void CreateResource(DomainResource resource);

    DomainResource ReadResource(string id);
  }
}