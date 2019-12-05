namespace Pact.Fhir.Mobile.Entities
{
  using System.Linq;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.SqlLite.Encryption;

  using SQLite;

  using Tangle.Net.Mam.Services;

  public class EncryptedResourceEntry
  {
    [NotNull]
    public string Channel { get; set; }

    [PrimaryKey, NotNull]
    public string Id { get; set; }

    [NotNull]
    public string Roots { get; set; }

    [NotNull]
    public string Subscription { get; set; }

    public static EncryptedResourceEntry FromResourceEntry(ResourceEntry resourceEntry, IEncryption encryption)
    {
      return new EncryptedResourceEntry
               {
                 Channel = encryption.Encrypt(resourceEntry.ChannelToJson()),
                 Subscription = encryption.Encrypt(resourceEntry.SubscriptionToJson()),
                 Roots = string.Join(";", resourceEntry.ResourceRoots),
                 Id = resourceEntry.ResourceRoots[0].Substring(0, 64)
               };
    }

    public ResourceEntry ToResourceEntry(IEncryption encryption, MamChannelFactory channelFactory, MamChannelSubscriptionFactory subscriptionFactory)
    {
      var decryptedChannel = encryption.Decrypt(this.Channel);
      var decryptedSubscription = encryption.Decrypt(this.Subscription);

      return new ResourceEntry
               {
                 ResourceRoots = this.Roots.Split(';').ToList(),
                 Channel = string.IsNullOrEmpty(decryptedChannel) ? null : channelFactory.CreateFromJson(decryptedChannel),
                 Subscription = subscriptionFactory.CreateFromJson(decryptedSubscription)
               };
    }
  }
}