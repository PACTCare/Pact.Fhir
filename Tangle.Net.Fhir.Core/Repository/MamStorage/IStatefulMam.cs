namespace Tangle.Net.Fhir.Core.Repository.MamStorage
{
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;

  /// <summary>
  /// The StatefulMam interface.
  /// </summary>
  public interface IStatefulMam
  {
    /// <summary>
    /// The add channel.
    /// </summary>
    /// <param name="channel">
    /// The channel.
    /// </param>
    void AddChannel(MamChannel channel);

    /// <summary>
    /// The add channel subscription.
    /// </summary>
    /// <param name="subscription">
    /// The subscription.
    /// </param>
    void AddChannelSubscription(MamChannelSubscription subscription);

    /// <summary>
    /// The get channel.
    /// </summary>
    /// <param name="seed">
    /// The seed.
    /// </param>
    /// <returns>
    /// The <see cref="MamChannel"/>.
    /// </returns>
    MamChannel GetChannel(Seed seed);

    /// <summary>
    /// The get subscription.
    /// </summary>
    /// <param name="root">
    /// The root.
    /// </param>
    /// <returns>
    /// The <see cref="MamChannelSubscription"/>.
    /// </returns>
    MamChannelSubscription GetSubscription(Hash root);
  }
}