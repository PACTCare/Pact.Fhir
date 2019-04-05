namespace Pact.Fhir.Iota.Tests.Services
{
  using System.Collections.Generic;
  using System.Linq;

  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Entity;

  public class InMemoryReferenceResolver : IReferenceResolver
  {
    public InMemoryReferenceResolver()
    {
      this.References = new Dictionary<string, Seed>();
    }

    public Dictionary<string, Seed> References { get; set; }

    /// <inheritdoc />
    public void AddReference(string reference, Seed seed)
    {
      this.References.Add(reference, seed);
    }

    /// <inheritdoc />
    public Seed Resolve(string reference)
    {
      return this.References.FirstOrDefault(e => e.Key == reference).Value;
    }
  }
}