namespace Pact.Fhir.Iota.Entity
{
  using Tangle.Net.Entity;

  public class ChannelCredentials
  {
    public string ChannelKey { get; set; }

    public Seed Seed { get; set; }
  }
}