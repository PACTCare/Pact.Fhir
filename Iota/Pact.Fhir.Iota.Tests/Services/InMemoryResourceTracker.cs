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
    public async Task<ResourceEntry> GetEntryAsync(string id)
    {
      return this.Entries.FirstOrDefault(e => e.ResourceIds.Any(h => h.Contains(id)));
    }

    /// <inheritdoc />
    public async Task UpdateEntryAsync(ResourceEntry entry)
    {
      var existingEntry = this.Entries.FirstOrDefault(e => e.ResourceIds.Any(h => h.Contains(entry.ResourceIds.First())));
      if (existingEntry != null)
      {
        existingEntry.ResourceIds = entry.ResourceIds;
        existingEntry.Channel = entry.Channel;
        existingEntry.Subscription = entry.Subscription;
      }
    }
  }
}