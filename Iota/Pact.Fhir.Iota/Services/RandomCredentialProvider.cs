namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;

  using Tangle.Net.Entity;

  public class RandomChannelCredentialProvider : IChannelCredentialProvider
  {
    /// <inheritdoc />
    public async Task<ChannelCredentials> CreateAsync()
    {
      return new ChannelCredentials { ChannelKey = Seed.Random().Value, Seed = Seed.Random() };
    }
  }
}