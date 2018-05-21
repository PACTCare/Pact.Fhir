namespace Tangle.Net.Fhir.Core.Repository.MamStorage
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;

  /// <summary>
  /// The memory cache stateful mam.
  /// </summary>
  public class MemoryCacheStatefulMam : IStatefulMam
  {
    /// <summary>
    /// Gets or sets the channels.
    /// </summary>
    private static List<MamChannel> Channels { get; set; }

    /// <summary>
    /// Gets or sets the subscriptions.
    /// </summary>
    private static List<MamChannelSubscription> Subscriptions { get; set; }

    /// <inheritdoc />
    public async Task AddChannel(MamChannel channel)
    {
      if (Channels == null)
      {
        Channels = new List<MamChannel>();
      }

      Channels.Add(channel);
    }

    /// <inheritdoc />
    public async Task AddChannelSubscription(MamChannelSubscription subscription)
    {
      if (Subscriptions == null)
      {
        Subscriptions = new List<MamChannelSubscription>();
      }

      Subscriptions.Add(subscription);
    }

    /// <inheritdoc />
    public async Task<MamChannel> GetChannel(Seed seed)
    {
      if (Channels == null)
      {
        Channels = new List<MamChannel>();
      }

      return Channels.FirstOrDefault(c => c.Seed.Value == seed.Value);
    }

    /// <inheritdoc />
    public async Task<MamChannelSubscription> GetSubscription(Hash root)
    {
      if (Subscriptions == null)
      {
        Subscriptions = new List<MamChannelSubscription>();
      }

      return Subscriptions.FirstOrDefault(s => s.MessageRoot.Value == root.Value);
    }
  }
}