namespace Pact.Fhir.Iota.SqlLite.Services
{
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;

  using Tangle.Net.Mam.Services;

  /// <summary>
  /// Basic example implementation of the resource tracker.
  /// Yes, the SQL could be optimized :-)
  /// </summary>
  public class SqlLiteResourceTracker : IResourceTracker
  {
    public SqlLiteResourceTracker(
      MamChannelFactory channelFactory,
      MamChannelSubscriptionFactory subscriptionFactory,
      IEncryption encryption,
      string databaseFilename = "iotafhir.sqlite")
    {
      this.ChannelFactory = channelFactory;
      this.SubscriptionFactory = subscriptionFactory;
      this.Encryption = encryption;
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      DatabaseInitializer.Init(databaseFilename);
    }

    public IEncryption Encryption { get; }

    private MamChannelFactory ChannelFactory { get; }

    private string ConnectionString { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task AddEntryAsync(ResourceEntry entry)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();
        long resourceId;

        var encryptedChannel = this.Encryption.Encrypt(entry.Channel.ToJson());
        var encryptedSubscription = this.Encryption.Encrypt(entry.Subscription.ToJson());

        using (var command = new SQLiteCommand(
          $"INSERT INTO Resource (Channel, Subscription) VALUES ('{encryptedChannel}', '{encryptedSubscription}'); SELECT last_insert_rowid();",
          connection))
        {
          resourceId = (long)await command.ExecuteScalarAsync();
        }

        foreach (var hash in entry.ResourceRoots)
        {
          using (var command = new SQLiteCommand(
            $"INSERT INTO StreamHash (Hash, ResourceId) VALUES ('{hash}', '{resourceId}')",
            connection))
          {
            await command.ExecuteNonQueryAsync();
          }
        }
      }
    }

    /// <inheritdoc />
    public async Task<ResourceEntry> GetEntryAsync(string id)
    {
      ResourceEntry entry = null;
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        long resourceId;
        using (var command = new SQLiteCommand($"SELECT * FROM StreamHash WHERE Hash LIKE '{id}%'", connection))
        {
          using (var result = await command.ExecuteReaderAsync())
          {
            if (result.Read())
            {
              resourceId = (long)result["ResourceId"];
            }
            else
            {
              return null;
            }
          }
        }

        var resourceIds = new List<string>();
        using (var command = new SQLiteCommand($"SELECT * FROM StreamHash WHERE ResourceId = '{resourceId}'", connection))
        {
          using (var result = await command.ExecuteReaderAsync())
          {
            while (result.Read())
            {
              resourceIds.Add(result["Hash"].ToString());
            }
          }
        }

        using (var command = new SQLiteCommand($"SELECT * FROM Resource WHERE Id = '{resourceId}'", connection))
        {
          using (var result = await command.ExecuteReaderAsync())
          {
            if (result.Read())
            {
              var decryptedChannel = this.Encryption.Decrypt(result["Channel"].ToString());
              var decryptedSubscription = this.Encryption.Decrypt(result["Subscription"].ToString());

              entry = new ResourceEntry
                        {
                          ResourceRoots = resourceIds,
                          Channel = this.ChannelFactory.CreateFromJson(decryptedChannel),
                          Subscription = this.SubscriptionFactory.CreateFromJson(decryptedSubscription)
                        };
            }
          }
        }
      }

      return entry;
    }

    /// <inheritdoc />
    public async Task UpdateEntryAsync(ResourceEntry entry)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        long resourceId;
        using (var command = new SQLiteCommand($"SELECT ResourceId FROM StreamHash WHERE Hash LIKE '{entry.ResourceRoots.First()}%'", connection))
        {
          using (var result = await command.ExecuteReaderAsync())
          {
            if (result.Read())
            {
              resourceId = (long)result["ResourceId"];
            }
            else
            {
              throw new ResourceNotFoundException(entry.ResourceRoots.First());
            }
          }
        }

        foreach (var hash in entry.ResourceRoots)
        {
          using (var command = new SQLiteCommand(
            $"INSERT OR IGNORE INTO StreamHash (Hash, ResourceId) VALUES ('{hash}', '{resourceId}')",
            connection))
          {
            await command.ExecuteNonQueryAsync();
          }
        }

        var encryptedChannel = this.Encryption.Encrypt(entry.Channel.ToJson());
        var encryptedSubscription = this.Encryption.Encrypt(entry.Subscription.ToJson());

        using (var command = new SQLiteCommand(
          $"UPDATE Resource SET Channel='{encryptedChannel}', Subscription='{encryptedSubscription}' WHERE Id={resourceId};",
          connection))
        {
          await command.ExecuteNonQueryAsync();
        }
      }
    }
  }
}