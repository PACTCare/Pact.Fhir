namespace Pact.Fhir.Iota.Tests.Services
{
  using System.Collections.Generic;

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
    public TryteString GetChannelKey(Hash versionId)
    {
      return null;
    }
  }
}