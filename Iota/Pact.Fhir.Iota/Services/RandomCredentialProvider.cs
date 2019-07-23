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
    public RandomSeedManager()
    {
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
    public Task<string> ImportChannelReadAccessAsync(string root, string channelKey)
    {
      return null;
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