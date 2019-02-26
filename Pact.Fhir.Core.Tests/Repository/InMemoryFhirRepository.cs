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

  using Task = System.Threading.Tasks.Task;

  public class InMemoryFhirRepository : IFhirRepository
  {
    public InMemoryFhirRepository(string creationId = null)
    {
      this.CreationId = creationId;
      this.Resources = new List<Resource>();
    }

    public List<Resource> Resources { get; }

    private string CreationId { get; }

    /// <inheritdoc />
    public async Task<Resource> CreateResourceAsync(Resource resource)
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
    public async Task DeleteResourceAsync(string id)
    {
      this.Resources.RemoveAll(r => id.Contains(r.Id));
    }

    /// <inheritdoc />
    public List<CapabilityStatement.ResourceComponent> GetCapabilities()
    {
      return new List<CapabilityStatement.ResourceComponent>();
    }

    /// <inheritdoc />
    public async Task<Resource> ReadResourceAsync(string id)
    {
      var resource = this.Resources.LastOrDefault(r => r.Id == id);
      if (resource == null)
      {
        throw new ResourceNotFoundException(id);
      }

      return resource;
    }

    /// <inheritdoc />
    public async Task<List<Resource>> ReadResourceHistoryAsync(string id)
    {
      return this.Resources.Where(r => r.Id == id).ToList();
    }

    /// <inheritdoc />
    public async Task<Resource> ReadResourceVersionAsync(string versionId)
    {
      var resource = this.Resources.LastOrDefault(r => r.VersionId == versionId);
      if (resource == null)
      {
        throw new ResourceNotFoundException(versionId);
      }

      return resource;
    }

    /// <inheritdoc />
    public async Task<Resource> UpdateResourceAsync(Resource resource)
    {
      var versionId = "SOMENEWVERSIONID";
      resource.PopulateMetadata(resource.Id, versionId);

      this.Resources.Add(resource);

      return resource;
    }
  }
}