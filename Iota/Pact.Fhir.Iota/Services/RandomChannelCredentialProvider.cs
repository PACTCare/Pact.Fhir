namespace Pact.Fhir.Iota.Services
{
  using Pact.Fhir.Iota.Entity;

  using Tangle.Net.Entity;

  public class RandomChannelCredentialProvider : IChannelCredentialProvider
  {
    /// <inheritdoc />
    public ChannelCredentials Create()
    {
      return new ChannelCredentials { ChannelKey = Seed.Random().Value, Seed = Seed.Random() };
    }
  }
}