namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;

  public abstract class DeterministicCredentialProvider : IChannelCredentialProvider
  {
    protected DeterministicCredentialProvider(
      Seed masterSeed,
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator)
    {
      this.MasterSeed = masterSeed;
      this.ResourceTracker = resourceTracker;
      this.SigningHelper = signingHelper;
      this.AddressGenerator = addressGenerator;
    }

    private IAddressGenerator AddressGenerator { get; }

    private Seed MasterSeed { get; }

    private IResourceTracker ResourceTracker { get; }

    private ISigningHelper SigningHelper { get; }

    /// <inheritdoc />
    public async Task<ChannelCredentials> CreateAsync()
    {
      var subSeed = new Seed(Converter.TritsToTrytes(this.SigningHelper.GetSubseed(this.MasterSeed, await this.GetNextSubSeedIndexAsync())));

      return new ChannelCredentials
               {
                 Seed = new Seed((await this.AddressGenerator.GetAddressAsync(subSeed, SecurityLevel.Low, 0)).Value),
                 ChannelKey = (await this.AddressGenerator.GetAddressAsync(subSeed, SecurityLevel.Low, 1)).Value
               };
    }

    protected abstract Task<int> GetCurrentSubSeedIndexAsync();

    protected abstract Task SetCurrentSubSeedIndexAsync(int index);

    private async Task<int> GetNextSubSeedIndexAsync()
    {
      var index = await this.GetCurrentSubSeedIndexAsync() + 1;

      // TODO: check if index was used from an other application

      await this.SetCurrentSubSeedIndexAsync(index);

      return index;
    }
  }
}