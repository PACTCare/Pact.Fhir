namespace Pact.Fhir.Core.Tests.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

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
    public async Task<DomainResource> CreateResourceAsync(DomainResource resource)
    {
      this.Resources.Add(resource);

      var resourceId = Guid.NewGuid();
      resource.Id = resourceId.ToString();
      resource.VersionId = resourceId.ToString();

      return resource;
    }

    /// <inheritdoc />
    public Task<DomainResource> ReadResourceAsync(string id)
    {
      return null;
    }
  }
}