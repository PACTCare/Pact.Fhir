namespace Pact.Fhir.Iota.Tests.Services
{
  using System.Collections.Generic;
  using System.Linq;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Entity;

  internal class InMemoryResourceTracker : IResourceTracker
  {
    public InMemoryResourceTracker()
    {
      this.Entries = new List<ResourceEntry>();
    }

    public List<ResourceEntry> Entries { get; }

    /// <inheritdoc />
    public void AddEntry(ResourceEntry entry)
    {
      this.Entries.Add(entry);
    }

    /// <inheritdoc />
    public ResourceEntry GetEntry(string versionId)
    {
      return this.Entries.FirstOrDefault(e => e.MerkleRoots.Any(h => h.Value.Contains(versionId)));
    }
  }
}