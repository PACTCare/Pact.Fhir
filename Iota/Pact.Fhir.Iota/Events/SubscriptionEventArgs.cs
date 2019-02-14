namespace Pact.Fhir.Iota.Events
{
  using System;

  using Tangle.Net.Mam.Entity;

  public class SubscriptionEventArgs : EventArgs
  {
    public SubscriptionEventArgs(MamChannelSubscription subscription)
    {
      this.Subscription = subscription;
    }

    public MamChannelSubscription Subscription { get; }
  }
}