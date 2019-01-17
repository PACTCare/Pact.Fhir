namespace Pact.Fhir.Iota.Entity
{
  using System.Collections.Generic;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;

  /// <summary>
  /// Since the restricted MAM mode is used, we need to keep track of the channel belonging and the subscription
  /// </summary>
  public class ResourceEntry
  {
    public ResourceEntry()
    {
      this.ResourceIds = new List<string>();
    }

    public MamChannel Channel { get; set; }

    public List<string> ResourceIds { get; set; }

    public MamChannelSubscription Subscription { get; set; }
  }
}