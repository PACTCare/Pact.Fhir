namespace Pact.Fhir.Iota.Services
{
  using Pact.Fhir.Iota.Entity;

  public interface IChannelCredentialProvider
  {
    ChannelCredentials Create();
  }
}