namespace Pact.Fhir.Iota.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Repository;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Merkle;

  public class RandomSeedManager : ISeedManager
  {
    private IResourceTracker ResourceTracker { get; }

    public RandomSeedManager()
    {
      this.References = new Dictionary<string, Seed>();
    }

    public RandomSeedManager(IResourceTracker resourceTracker)
    {
      this.ResourceTracker = resourceTracker;
      this.References = new Dictionary<string, Seed>();
    }

    public int CurrentIndex { get; set; }

    public Dictionary<string, Seed> References { get; set; }

    /// <inheritdoc />
    public async Task AddReferenceAsync(string reference, Seed seed)
    {
      this.References.Add(reference, seed);
    }

    /// <inheritdoc />
    public async Task DeleteReferenceAsync(string reference)
    {
      this.References.Remove(reference);
    }

    /// <inheritdoc />
    public async Task<ChannelCredentials> CreateChannelCredentialsAsync(Seed seed)
    {
      return new ChannelCredentials
               {
                 ChannelKey = Seed.Random().Value,
                 Seed = seed,
                 RootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, IotaFhirRepository.SecurityLevel).Root.Hash
               };
    }

    /// <inheritdoc />
    public async Task<string> ImportChannelReadAccessAsync(string root, string channelKey)
    {
      await this.ResourceTracker.AddEntryAsync(new ResourceEntry { ResourceRoots = new List<string> { root } });

      return root.Substring(0, 64);
    }

    /// <inheritdoc />
    public Task ImportChannelWriteAccessAsync(ChannelCredentials credentials)
    {
      return null;
    }

    /// <inheritdoc />
    public async Task<Seed> ResolveReferenceAsync(string reference = null)
    {
      return this.References.FirstOrDefault(e => e.Key == reference).Value;
    }

    /// <inheritdoc />
    public async Task SyncAsync(Seed seed)
    {
    }
  }
}