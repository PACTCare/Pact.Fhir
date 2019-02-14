namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;

  public interface IChannelCredentialProvider
  {
    Task<ChannelCredentials> CreateAsync();

    Task SyncAsync();
  }
}