namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;

  using Tangle.Net.Entity;

  public interface IChannelCredentialProvider
  {
    Task<ChannelCredentials> CreateAsync(Seed seed);

    Task SyncAsync(Seed seed);
  }
}