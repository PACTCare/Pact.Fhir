namespace Pact.Fhir.Core.Repository
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Task = System.Threading.Tasks.Task;

  public interface ISearchRepository
  {
    Task AddResourceAsync(Resource resource);

    Task<List<Resource>> FindResourcesByTypeAsync(string typeName);

    Task UpdateResourceAsync(Resource resource);

    Task DeleteResourceAsync(string resourceId);
  }
}