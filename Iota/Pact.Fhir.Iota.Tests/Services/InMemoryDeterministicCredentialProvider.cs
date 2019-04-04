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
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator,
      IIotaRepository repository)
      : base(resourceTracker, signingHelper, addressGenerator, repository)
    {
      this.CurrentIndex = -1;
    }

    public int CurrentIndex { get; set; }

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