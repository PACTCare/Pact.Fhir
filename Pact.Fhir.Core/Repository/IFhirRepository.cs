namespace Pact.Fhir.Core.Repository
{
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  public interface IFhirRepository
  {
    Task<DomainResource> CreateResourceAsync(DomainResource resource);

    Task<DomainResource> ReadResourceAsync(string id);
  }
}