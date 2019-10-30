namespace Pact.Fhir.Core.Repository
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Task = System.Threading.Tasks.Task;

  public class InMemorySearchRepository : ISearchRepository
  {
    public InMemorySearchRepository()
    {
      this.Resources = new List<Resource>();
    }

    private List<Resource> Resources { get; }

    /// <inheritdoc />
    public async Task AddResourceAsync(Resource resource)
    {
      if (this.Resources.All(r => r.Id != resource.Id))
      {
        this.Resources.Add(resource);
      }
    }

    /// <inheritdoc />
    public async Task<List<Resource>> FindResourcesByTypeAsync(string typeName)
    {
      return this.Resources.Where(r => r.TypeName == typeName).ToList();
    }

    /// <inheritdoc />
    public async Task UpdateResourceAsync(Resource resource)
    {
      var resourceToUpdate = this.Resources.FirstOrDefault(r => r.Id == resource.Id);

      if (resourceToUpdate != null)
      {
        resourceToUpdate = resource;
      }
    }

    /// <inheritdoc />
    public async Task DeleteResourceAsync(string resourceId)
    {
      this.Resources.Remove(this.Resources.First(r => r.Id == resourceId));
    }
  }
}