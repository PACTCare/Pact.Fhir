namespace Pact.Fhir.Mobile.Repository
{
  using System.IO;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;
  using Pact.Fhir.Mobile.Entities;

  using SQLite;

  using Tangle.Net.Mam.Services;

  public class ResourceTracker : IResourceTracker
  {
    public ResourceTracker(
      MamChannelFactory channelFactory,
      MamChannelSubscriptionFactory subscriptionFactory,
      IEncryption encryption,
      string databaseFilename = "iotafhir.sqlite")
    {
      this.ChannelFactory = channelFactory;
      this.SubscriptionFactory = subscriptionFactory;
      this.Encryption = encryption;
      this.ConnectionString = databaseFilename;

      this.Init(databaseFilename);
    }

    private void Init(string databaseFilename)
    {
      if (File.Exists(databaseFilename))
      {
        return;
      }

      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.CreateTable<EncryptedResourceEntry>();
      }
    }

    private IEncryption Encryption { get; }

    private MamChannelFactory ChannelFactory { get; }

    private string ConnectionString { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task AddEntryAsync(ResourceEntry entry)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.InsertOrReplaceAsync(EncryptedResourceEntry.FromResourceEntry(entry, this.Encryption));
    }

    /// <inheritdoc />
    public async Task<ResourceEntry> GetEntryAsync(string id)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      var entry = await connection.FindAsync<EncryptedResourceEntry>(id);

      return entry.ToResourceEntry(this.Encryption, this.ChannelFactory, this.SubscriptionFactory);
    }

    /// <inheritdoc />
    public async Task UpdateEntryAsync(ResourceEntry entry)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.UpdateAsync(EncryptedResourceEntry.FromResourceEntry(entry, this.Encryption));
    }

    /// <inheritdoc />
    public async Task DeleteEntryAsync(string id)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.DeleteAsync(id);
    }

    public async Task DeleteAllAsync()
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.DeleteAllAsync<EncryptedResourceEntry>();
    }
  }
}