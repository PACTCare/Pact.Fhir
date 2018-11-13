namespace Pact.Fhir.Core.Services.Cache
{
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  /// <summary>
  /// The in memory patient data cache.
  /// </summary>
  public class InMemoryResourceCache : IResourceCache
  {
    /// <summary>
    /// Gets or sets the cache data.
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
    public void SetResource<T>(T resource, Hash root)
      where T : DomainResource
    {
      if (CacheData == null)
      {
        CacheData = new Dictionary<string, DomainResource>();
      }

      CacheData.Add(root.Value, resource);
    }

    /// <inheritdoc />
    public bool IsSet(Hash root)
    {
      if (CacheData == null)
      {
        CacheData = new Dictionary<string, DomainResource>();
      }

      return CacheData.FirstOrDefault(c => c.Key == root.Value).Value != null;
    }
  }
}