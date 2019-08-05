namespace Pact.Fhir.Iota.Events
{
  using System;

  public class SubscriptionAddedEventArgs : EventArgs
  {
    public SubscriptionAddedEventArgs(string resourceId)
    {
      this.ResourceId = resourceId;
    }

    public string ResourceId { get; }
  }
}