namespace Pact.Fhir.Iota.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Repository;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.Repository;

  public abstract class DeterministicSeedManager : ISeedManager
  {
    private const int ChannelKeyIndex = 1;

    private const int ChannelSeedIndex = 0;

    protected DeterministicSeedManager(
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator,
      IIotaRepository repository)
    {
      this.ResourceTracker = resourceTracker;
      this.SigningHelper = signingHelper;
      this.AddressGenerator = addressGenerator;
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
      this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, repository);
    }

    private IAddressGenerator AddressGenerator { get; }

    private MamChannelFactory ChannelFactory { get; }

    private IResourceTracker ResourceTracker { get; }

    private ISigningHelper SigningHelper { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public abstract Task AddReferenceAsync(string reference, Seed seed);

    /// <inheritdoc />
    public async Task<ChannelCredentials> CreateChannelCredentialsAsync(Seed seed)
    {
      // Create new channel credentials with the current index incremented by one
      return await this.FindAndUpdateCurrentIndexAsync(seed, await this.GetCurrentSubSeedIndexAsync(seed) + 1);
    }

    /// <inheritdoc />
    public async Task<string> ImportChannelReadAccessAsync(string root, string channelKey)
    {
      var subscription = this.SubscriptionFactory.Create(new Hash(root), Mode.Restricted, channelKey, true);

      await this.ResourceTracker.AddEntryAsync(new ResourceEntry { ResourceRoots = new List<string> { root }, Subscription = subscription });

      return root.Substring(0, 64);
    }

    /// <inheritdoc />
    public async Task ImportChannelWriteAccessAsync(ChannelCredentials credentials)
    {
      var subscription = this.SubscriptionFactory.Create(credentials.RootHash, Mode.Restricted, credentials.ChannelKey, true);
      var channel = this.ChannelFactory.Create(Mode.Restricted, credentials.Seed, IotaFhirRepository.SecurityLevel, credentials.ChannelKey);

      await this.ResourceTracker.AddEntryAsync(
        new ResourceEntry { ResourceRoots = new List<string> { credentials.RootHash.Value }, Subscription = subscription, Channel = channel });
    }

    /// <inheritdoc />
    public abstract Task<Seed> ResolveReferenceAsync(string reference = null);

    /// <inheritdoc />
    public async Task SyncAsync(Seed seed)
    {
      // Start sync with seed at index 1 (lowest index possible)
      await this.FindAndUpdateCurrentIndexAsync(seed, 1);
    }

    protected abstract Task<int> GetCurrentSubSeedIndexAsync(Seed seed);

    protected abstract Task SetCurrentSubSeedIndexAsync(Seed seed, int index);

    private async Task<ChannelCredentials> FindAndUpdateCurrentIndexAsync(Seed seed, int index)
    {
      while (true)
      {
        var subSeed = new Seed(Converter.TritsToTrytes(this.SigningHelper.GetSubseed(seed, index)));
        var channelSeed = new Seed((await this.AddressGenerator.GetAddressAsync(subSeed, SecurityLevel.Low, ChannelSeedIndex)).Value);
        var channelKey = (await this.AddressGenerator.GetAddressAsync(subSeed, SecurityLevel.Low, ChannelKeyIndex)).Value;

        var rootHash = CurlMerkleTreeFactory.Default.Create(channelSeed, 0, 1, IotaFhirRepository.SecurityLevel).Root.Hash;

        // Check if the index was used by another application. If not, return the corresponding channel credentials
        var subscription = this.SubscriptionFactory.Create(rootHash, Mode.Restricted, channelKey);
        var message = await subscription.FetchSingle(rootHash);
        if (message == null)
        {
          await this.SetCurrentSubSeedIndexAsync(seed, index);

          var credentials = new ChannelCredentials { Seed = channelSeed, ChannelKey = channelKey, RootHash = rootHash };
          await this.ImportChannelWriteAccessAsync(credentials);

          return credentials;
        }

        // The index is already in use. Increment by one and check that in the next round of the loop
        index++;
      }
    }
  }
}