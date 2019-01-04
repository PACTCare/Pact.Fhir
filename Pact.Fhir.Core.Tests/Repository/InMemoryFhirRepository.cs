namespace Pact.Fhir.Core.Tests.Repository
{
  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;

  public class InMemoryFhirRepository : IFhirRepository
  {
    /// <inheritdoc />
    public void CreateResource(DomainResource resource)
    {
    }

    /// <inheritdoc />
    public DomainResource ReadResource(string id)
    {
      return null;
    }
  }
}