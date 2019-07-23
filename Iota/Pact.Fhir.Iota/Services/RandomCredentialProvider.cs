namespace Pact.Fhir.Iota.Services
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Events;
  using Pact.Fhir.Iota.Repository;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Merkle;

  public class RandomSeedManager : ISeedManager
  {
    /// <inheritdoc />
    public event EventHandler<SubscriptionEventArgs> SubscriptionFound;

    /// <inheritdoc />
    public async Task<ChannelCredentials> CreateAsync(Seed seed)
    {
      return new ChannelCredentials
               {
                 ChannelKey = Seed.Random().Value,
                 Seed = seed,
                 RootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, IotaFhirRepository.SecurityLevel).Root.Hash
               };
    }

    /// <inheritdoc />
    public Task<Seed> ExportSeed(string reference = null)
    {
      return null;
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
    public async Task SyncAsync(Seed seed)
    {
    }
  }
}