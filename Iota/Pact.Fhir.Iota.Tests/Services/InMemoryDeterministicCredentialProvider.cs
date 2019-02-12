namespace Pact.Fhir.Iota.Tests.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;
  using Tangle.Net.Repository;

  public class InMemoryDeterministicCredentialProvider : DeterministicCredentialProvider
  {
    /// <inheritdoc />
    public InMemoryDeterministicCredentialProvider(
      Seed masterSeed,
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator,
      IIotaRepository repository)
      : base(masterSeed, resourceTracker, signingHelper, addressGenerator, repository)
    {
      this.CurrentIndex = -1;
    }

    public int CurrentIndex { get; set; }

    /// <inheritdoc />
    protected override async Task<int> GetCurrentSubSeedIndexAsync()
    {
      return this.CurrentIndex;
    }

    /// <inheritdoc />
    protected override async Task SetCurrentSubSeedIndexAsync(int index)
    {
      this.CurrentIndex = index;
    }
  }
}