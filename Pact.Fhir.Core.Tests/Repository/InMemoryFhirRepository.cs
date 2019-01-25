namespace Pact.Fhir.Core.Tests.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  public class InMemoryFhirRepository : IFhirRepository
  {
    public InMemoryFhirRepository(string creationId = null)
    {
      this.CreationId = creationId;
      this.Resources = new List<DomainResource>();
    }

    public List<DomainResource> Resources { get; }

    private string CreationId { get; }

    /// <inheritdoc />
    public async Task<DomainResource> CreateResourceAsync(DomainResource resource)
    {
      this.Resources.Add(resource);

      string resourceId;
      if (this.CreationId != null)
      {
        resourceId = this.CreationId;
      }
      else
      {
        resourceId = "SOMEFHIRCONFORMID1234";
      }

      resource.PopulateMetadata(resourceId, resourceId);

      return resource;
    }

    /// <inheritdoc />
    public async Task<DomainResource> ReadResourceAsync(string id)
    {
      var resource = this.Resources.FirstOrDefault(r => r.Id == id);

      if (resource == null)
      {
        throw new ResourceNotFoundException(id);
      }

      return resource;
    }

    /// <inheritdoc />
    public Task<DomainResource> UpdateResourceAsync(DomainResource resource)
    {
      return null;
    }
  }
}