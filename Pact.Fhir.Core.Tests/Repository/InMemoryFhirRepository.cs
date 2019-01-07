namespace Pact.Fhir.Core.Tests.Repository
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;

  public class InMemoryFhirRepository : IFhirRepository
  {
    public InMemoryFhirRepository()
    {
      this.Resources = new List<DomainResource>();
    }

    public List<DomainResource> Resources { get; }

    /// <inheritdoc />
    public void CreateResource(DomainResource resource)
    {
      this.Resources.Add(resource);
    }

    /// <inheritdoc />
    public DomainResource ReadResource(string id)
    {
      return null;
    }
  }
}