namespace Pact.Fhir.Core.Repository
{
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  /// <summary>
  /// The in memory patient data repository.
  /// </summary>
  public class InMemoryResourceRepository : IResourceRepository
  {
    /// <summary>
    /// Gets or sets the repository data.
    /// </summary>
    private static Dictionary<string, DomainResource> CacheData { get; set; }

    /// <inheritdoc />
    public T GetResourceByRoot<T>(Hash root)
      where T : DomainResource
    {
      if (CacheData == null)
      {
        CacheData = new Dictionary<string, DomainResource>();
      }

      return (T)CacheData.FirstOrDefault(c => c.Key == root.Value).Value;
    }

    /// <inheritdoc />
    public void SaveResource<T>(T resource, Hash root, int index, int patientId)
      where T : DomainResource
    {
      if (CacheData == null)
      {
        CacheData = new Dictionary<string, DomainResource>();
      }

      CacheData.Add(root.Value, resource);
    }
  }
}