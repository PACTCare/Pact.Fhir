namespace Pact.Fhir.Iota.Entity
{
  using System.Collections.Generic;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Services;

  /// <summary>
  /// Since the restricted MAM mode is used, we need to keep track of the channel belonging and the subscription
  /// </summary>
  public class ResourceEntry
  {
    public ResourceEntry()
    {
      this.StreamHashes = new List<Hash>();
    }

    public string Channel { get; set; }

    public List<Hash> StreamHashes { get; set; }

    public string Subscription { get; set; }

    public MamChannel GetChannel(MamChannelFactory channelFactory)
    {
      return channelFactory.CreateFromJson(this.Channel);
    }

    public MamChannelSubscription GetSubscription(MamChannelSubscriptionFactory subscriptionFactory)
    {
      return subscriptionFactory.CreateFromJson(this.Subscription);
    }
  }
}