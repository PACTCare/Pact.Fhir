namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;

  using Tangle.Net.Entity;

  public interface ISeedManager
  {
    Task<ChannelCredentials> CreateAsync(Seed seed);

    Task<Seed> ExportSeed(string reference = null);

    Task<string> ImportChannelReadAccessAsync(string root, string channelKey);

    Task ImportChannelWriteAccessAsync(ChannelCredentials credentials);

    Task SyncAsync(Seed seed);
  }
}