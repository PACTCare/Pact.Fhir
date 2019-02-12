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
      var index = await this.GetCurrentSubSeedIndexAsync() + 1;

      while (true)
      {
        var subSeed = new Seed(Converter.TritsToTrytes(this.SigningHelper.GetSubseed(this.MasterSeed, index)));
        var seed = new Seed((await this.AddressGenerator.GetAddressAsync(subSeed, SecurityLevel.Low, ChannelSeedIndex)).Value);
        var channelKey = (await this.AddressGenerator.GetAddressAsync(subSeed, SecurityLevel.Low, ChannelKeyIndex)).Value;

        var rootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, IotaFhirRepository.SecurityLevel).Root.Hash;

        var message = await this.SubscriptionFactory.Create(rootHash, Mode.Restricted, channelKey).FetchSingle(rootHash);
        if (message == null)
        {
          await this.SetCurrentSubSeedIndexAsync(index);
          return new ChannelCredentials { Seed = seed, ChannelKey = channelKey, RootHash = rootHash };
        }

        index++;
      }
    }

    protected abstract Task<int> GetCurrentSubSeedIndexAsync();

    protected abstract Task SetCurrentSubSeedIndexAsync(int index);
  }
}