namespace Pact.Fhir.Iota.Services
{
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

  public abstract class DeterministicCredentialProvider : IChannelCredentialProvider
  {
    private const int ChannelKeyIndex = 1;

    private const int ChannelSeedIndex = 0;

    protected DeterministicCredentialProvider(
      Seed masterSeed,
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator,
      IIotaRepository repository)
    {
      this.MasterSeed = masterSeed;
      this.ResourceTracker = resourceTracker;
      this.SigningHelper = signingHelper;
      this.AddressGenerator = addressGenerator;
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
    }

    private IAddressGenerator AddressGenerator { get; }

    private Seed MasterSeed { get; }

    private IResourceTracker ResourceTracker { get; }

    private ISigningHelper SigningHelper { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task<ChannelCredentials> CreateAsync()
    {
      // Create new channel credentials with the current index incremented by one
      return await this.FindAndUpdateCurrentIndexAsync(await this.GetCurrentSubSeedIndexAsync() + 1);
    }

    /// <inheritdoc />
    public async Task SyncAsync()
    {
      // Start sync with seed at index 0 (lowest index possible)
      await this.FindAndUpdateCurrentIndexAsync(0);
    }

    protected abstract Task<int> GetCurrentSubSeedIndexAsync();

    protected abstract Task SetCurrentSubSeedIndexAsync(int index);

    private async Task<ChannelCredentials> FindAndUpdateCurrentIndexAsync(int index)
    {
      while (true)
      {
        var subSeed = new Seed(Converter.TritsToTrytes(this.SigningHelper.GetSubseed(this.MasterSeed, index)));
        var seed = new Seed((await this.AddressGenerator.GetAddressAsync(subSeed, SecurityLevel.Low, ChannelSeedIndex)).Value);
        var channelKey = (await this.AddressGenerator.GetAddressAsync(subSeed, SecurityLevel.Low, ChannelKeyIndex)).Value;

        var rootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, IotaFhirRepository.SecurityLevel).Root.Hash;

        // Check if the index was used by another application. If not, return the corresponding channel credentials
        var message = await this.SubscriptionFactory.Create(rootHash, Mode.Restricted, channelKey).FetchSingle(rootHash);
        if (message == null)
        {
          await this.SetCurrentSubSeedIndexAsync(index);
          return new ChannelCredentials { Seed = seed, ChannelKey = channelKey, RootHash = rootHash };
        }

        // The index is already in use. Increment by one and check that in the next round of the loop
        index++;
      }
    }
  }
}