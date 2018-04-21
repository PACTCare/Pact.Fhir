namespace Tangle.Net.Fhir.Core.Repository.MamStorage
{
  using System.Collections.Generic;
  using System.Linq;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;

  /// <summary>
  /// The memory cache stateful mam.
  /// </summary>
  public class MemoryCacheStatefulMam : IStatefulMam
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheStatefulMam"/> class.
    /// </summary>
    public MemoryCacheStatefulMam()
    {
      this.Channels = new List<MamChannel>();
      this.Subscriptions = new List<MamChannelSubscription>();
    }

    /// <summary>
    /// Gets the channels.
    /// </summary>
    private List<MamChannel> Channels { get; }

    /// <summary>
    /// Gets the subscriptions.
    /// </summary>
    private List<MamChannelSubscription> Subscriptions { get; }

    /// <inheritdoc />
    public void AddChannel(MamChannel channel)
    {
      this.Channels.Add(channel);
    }

    /// <inheritdoc />
    public void AddChannelSubscription(MamChannelSubscription subscription)
    {
      this.Subscriptions.Add(subscription);
    }

    /// <inheritdoc />
    public MamChannel GetChannel(Seed seed)
    {
      return this.Channels.FirstOrDefault(c => c.Seed.Value == seed.Value);
    }

    /// <inheritdoc />
    public MamChannelSubscription GetSubscription(Hash root)
    {
      return this.Subscriptions.FirstOrDefault(s => s.MessageRoot.Value == root.Value);
    }
  }
}