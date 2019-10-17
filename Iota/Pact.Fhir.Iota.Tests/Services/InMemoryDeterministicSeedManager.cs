namespace Pact.Fhir.Iota.Tests.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;
  using Tangle.Net.Repository;

  public class InMemoryDeterministicSeedManager : DeterministicSeedManager
  {
    /// <inheritdoc />
    public InMemoryDeterministicSeedManager(
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator,
      IIotaRepository repository)
      : base(resourceTracker, signingHelper, addressGenerator, repository)
    {
      this.CurrentIndex = -1;
      this.References = new Dictionary<string, Seed>();
    }

    public Dictionary<string, Seed> References { get; set; }

    public int CurrentIndex { get; set; }

    /// <inheritdoc />
    public override async Task AddReferenceAsync(string reference, Seed seed)
    {
      this.References.Add(reference, seed);
    }

    /// <inheritdoc />
    public override async Task DeleteReferenceAsync(string reference)
    {
      this.References.Remove(reference);
    }

    /// <inheritdoc />
    public override async Task<Seed> ResolveReferenceAsync(string reference = null)
    {
      return this.References.FirstOrDefault(e => e.Key == reference).Value;
    }

    /// <inheritdoc />
    protected override async Task<int> GetCurrentSubSeedIndexAsync(Seed seed)
    {
      return this.CurrentIndex;
    }

    /// <inheritdoc />
    protected override async Task SetCurrentSubSeedIndexAsync(Seed seed, int index)
    {
      this.CurrentIndex = index;
    }
  }
}