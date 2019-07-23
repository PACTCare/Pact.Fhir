namespace Pact.Fhir.Iota.Services
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Events;

  using Tangle.Net.Entity;

  public interface ISeedManager
  {
    Task<ChannelCredentials> CreateAsync(Seed seed);

    Task<string> ImportChannelReadAccessAsync(string root, string channelKey);

    Task ImportChannelWriteAccessAsync(ChannelCredentials credentials);

    Task SyncAsync(Seed seed);
  }
}