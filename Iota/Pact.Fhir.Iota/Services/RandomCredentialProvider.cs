namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Repository;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Merkle;

  public class RandomChannelCredentialProvider : IChannelCredentialProvider
  {
    /// <inheritdoc />
    public async Task<ChannelCredentials> CreateAsync()
    {
      var seed = Seed.Random();

      return new ChannelCredentials
               {
                 ChannelKey = Seed.Random().Value,
                 Seed = seed,
                 RootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, IotaFhirRepository.SecurityLevel).Root.Hash
               };
    }

    /// <inheritdoc />
    public async Task SyncAsync()
    {
    }
  }
}