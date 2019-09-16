namespace Pact.Fhir.Iota.Events
{
  using System;

  using Tangle.Net.Entity;

  public class SubscriptionAddedEventArgs : EventArgs
  {
    public SubscriptionAddedEventArgs(string resourceId, Seed seed)
    {
      this.ResourceId = resourceId;
      this.Seed = seed;
    }

    public string ResourceId { get; }
    public Seed Seed { get; }
  }
}