namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;

  using Tangle.Net.Entity;

  public interface ISeedManager
  {
    Task<ChannelCredentials> CreateChannelCredentialsAsync(Seed seed);

    Task<Seed> ResolveReferenceAsync(string reference = null);

    Task<string> ImportChannelReadAccessAsync(string root, string channelKey);

    Task ImportChannelWriteAccessAsync(ChannelCredentials credentials);

    Task SyncAsync(Seed seed);

    Task AddReferenceAsync(string reference, Seed seed);

    Task DeleteReferenceAsync(string reference);
  }
}