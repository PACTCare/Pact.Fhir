namespace Pact.Fhir.Iota.Entity
{
  using System.Collections.Generic;

  using Tangle.Net.Entity;

  /// <summary>
  /// Since the restricted MAM mode is used, we need to keep track of the channel keys belonging to certain roots
  /// </summary>
  public class ResourceEntry
  {
    public TryteString ChannelKey { get; set; }

    public List<Hash> MerkleRoots { get; set; }
  }
}