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
          "INSERT INTO Resource (Channel, Subscription) VALUES (@encryptedChannel, @encryptedSubscription); SELECT last_insert_rowid();",
          connection))
        {
          command.Parameters.AddWithValue("encryptedChannel", encryptedChannel);
          command.Parameters.AddWithValue("encryptedSubscription", encryptedSubscription);

          resourceId = (long)await command.ExecuteScalarAsync();
        }

        using (var transaction = connection.BeginTransaction())
        {
          foreach (var hash in entry.ResourceRoots)
          {
            using (var command = new SQLiteCommand(
              "INSERT OR IGNORE INTO StreamHash (Hash, ResourceId) VALUES (@hash, @resourceId)",
              connection,
              transaction))
            {
              command.Parameters.AddWithValue("hash", hash);
              command.Parameters.AddWithValue("resourceId", resourceId);

              await command.ExecuteNonQueryAsync();
            }
          }

          transaction.Commit();
        }
      }
    }

    /// <inheritdoc />
    public async Task<ResourceEntry> GetEntryAsync(string id)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        long resourceId;
        using (var command = new SQLiteCommand("SELECT * FROM StreamHash WHERE Hash LIKE @id", connection))
        {
          command.Parameters.AddWithValue("id", $"{id}%");

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
        using (var command = new SQLiteCommand("SELECT * FROM StreamHash WHERE ResourceId = @resourceId", connection))
        {
          command.Parameters.AddWithValue("resourceId", resourceId);

          using (var result = await command.ExecuteReaderAsync())
          {
            while (result.Read())
            {
              resourceIds.Add(result["Hash"].ToString());
            }
          }
        }

        using (var command = new SQLiteCommand("SELECT * FROM Resource WHERE Id = @resourceId", connection))
        {
          command.Parameters.AddWithValue("resourceId", resourceId);

          using (var result = await command.ExecuteReaderAsync())
          {
            if (!result.Read())
            {
              return null;
            }

            var decryptedChannel = this.Encryption.Decrypt(result["Channel"].ToString());
            var decryptedSubscription = this.Encryption.Decrypt(result["Subscription"].ToString());

            return new ResourceEntry
                      {
                        ResourceRoots = resourceIds,
                        Channel = this.ChannelFactory.CreateFromJson(decryptedChannel),
                        Subscription = this.SubscriptionFactory.CreateFromJson(decryptedSubscription)
                      };
          }
        }
      }
    }

    /// <inheritdoc />
    public async Task UpdateEntryAsync(ResourceEntry entry)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        long resourceId;
        using (var command = new SQLiteCommand("SELECT ResourceId FROM StreamHash WHERE Hash LIKE @hash", connection))
        {
          command.Parameters.AddWithValue("hash", $"{entry.ResourceRoots.First()}%");

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

        using (var transaction = connection.BeginTransaction())
        {
          foreach (var hash in entry.ResourceRoots)
          {
            using (var command = new SQLiteCommand(
              "INSERT OR IGNORE INTO StreamHash (Hash, ResourceId) VALUES (@hash, @resourceId)",
              connection,
              transaction))
            {
              command.Parameters.AddWithValue("hash", hash);
              command.Parameters.AddWithValue("resourceId", resourceId);

              await command.ExecuteNonQueryAsync();
            }
          }

          var encryptedChannel = this.Encryption.Encrypt(entry.Channel.ToJson());
          var encryptedSubscription = this.Encryption.Encrypt(entry.Subscription.ToJson());

          using (var command = new SQLiteCommand(
            $"UPDATE Resource SET Channel=@encryptedChannel, Subscription=@encryptedSubscription WHERE Id=@resourceId;",
            connection,
            transaction))
          {
            command.Parameters.AddWithValue("encryptedChannel", encryptedChannel);
            command.Parameters.AddWithValue("encryptedSubscription", encryptedSubscription);
            command.Parameters.AddWithValue("resourceId", resourceId);

            await command.ExecuteNonQueryAsync();
          }

          transaction.Commit();
        }
      }
    }
  }
}