namespace Pact.Fhir.Iota.Tests.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Services;

  internal class InMemoryResourceTracker : IResourceTracker
  {
    public InMemoryResourceTracker()
    {
      this.Entries = new List<ResourceEntry>();
    }

    public List<ResourceEntry> Entries { get; }

    /// <inheritdoc />
    public async Task AddEntryAsync(ResourceEntry entry)
    {
      this.Entries.Add(entry);
    }

    /// <inheritdoc />
    public async Task<ResourceEntry> GetEntryAsync(string versionId)
    {
      return this.Entries.FirstOrDefault(e => e.StreamHashes.Any(h => h.Value.Contains(versionId)));
    }
  }
}