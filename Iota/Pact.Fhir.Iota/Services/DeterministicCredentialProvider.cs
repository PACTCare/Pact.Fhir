namespace Pact.Fhir.Iota.Services
{
  using Pact.Fhir.Iota.Entity;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;

  public class DeterministicCredentialProvider : IChannelCredentialProvider
  {
    public DeterministicCredentialProvider(
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
    public ChannelCredentials Create()
    {
      var subSeed = new Seed(Converter.TritsToTrytes(this.SigningHelper.GetSubseed(this.MasterSeed, this.GetNextSubSeedIndex())));

      return new ChannelCredentials
               {
                 Seed = new Seed(this.AddressGenerator.GetAddress(subSeed, SecurityLevel.Low, 0).Value),
                 ChannelKey = this.AddressGenerator.GetAddress(subSeed, SecurityLevel.Low, 1).Value
               };
    }

    private int GetNextSubSeedIndex()
    {
      return 0;
    }
  }
}