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
      return this.Entries.FirstOrDefault(e => e.ResourceRoots.Any(h => h.Contains(id)));
    }

    /// <inheritdoc />
    public async Task UpdateEntryAsync(ResourceEntry entry)
    {
      var existingEntry = this.Entries.FirstOrDefault(e => e.ResourceRoots.Any(h => h.Contains(entry.ResourceRoots.First())));
      if (existingEntry != null)
      {
        existingEntry.ResourceRoots = entry.ResourceRoots;
        existingEntry.Channel = entry.Channel;
        existingEntry.Subscription = entry.Subscription;
      }
    }

    /// <inheritdoc />
    public async Task DeleteEntryAsync(string id)
    {
      this.Entries.Remove(this.Entries.First(e => e.ResourceRoots.Contains(id)));
    }
  }
}