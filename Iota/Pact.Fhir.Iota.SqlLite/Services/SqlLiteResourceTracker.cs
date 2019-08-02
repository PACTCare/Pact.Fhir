namespace Pact.Fhir.Iota.SqlLite.Services
{
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.SqlLite;
  using Pact.Fhir.Core.SqlLite.Repository;
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
      IDbConnectionSupplier connectionSupplier = null,
      string databaseFilename = "iotafhir.sqlite")
    {
      this.ChannelFactory = channelFactory;
      this.SubscriptionFactory = subscriptionFactory;
      this.Encryption = encryption;
      this.ConnectionSupplier = connectionSupplier ?? new DefaultDbConnectionSupplier();
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      DatabaseInitializer.InitFhirDatabase(this.ConnectionSupplier, databaseFilename);
    }

    private IEncryption Encryption { get; }
    private IDbConnectionSupplier ConnectionSupplier { get; }

    private MamChannelFactory ChannelFactory { get; }

    private string ConnectionString { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task AddEntryAsync(ResourceEntry entry)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();
        long resourceId;

        var encryptedChannel = this.Encryption.Encrypt(entry.ChannelToJson());
        var encryptedSubscription = this.Encryption.Encrypt(entry.SubscriptionToJson());

        using (var command = connection.CreateCommand())
        {
          command.CommandText =
            "INSERT INTO Resource (Channel, Subscription) VALUES (@encryptedChannel, @encryptedSubscription); SELECT last_insert_rowid();";

          command.AddWithValue("encryptedChannel", encryptedChannel);
          command.AddWithValue("encryptedSubscription", encryptedSubscription);

          resourceId = (long)await command.ExecuteScalarAsync();
        }

        using (var transaction = connection.BeginTransaction())
        {
          foreach (var hash in entry.ResourceRoots)
          {
            using (var command = connection.CreateCommand())
            {
              command.CommandText = "INSERT OR IGNORE INTO StreamHash (Hash, ResourceId) VALUES (@hash, @resourceId)";
              command.Transaction = transaction;

              command.AddWithValue("hash", hash);
              command.AddWithValue("resourceId", resourceId);

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
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        long resourceId;
        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT * FROM StreamHash WHERE Hash LIKE @id";
          command.AddWithValue("id", $"{id}%");

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
        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT * FROM StreamHash WHERE ResourceId = @resourceId";
          command.AddWithValue("resourceId", resourceId);

          using (var result = await command.ExecuteReaderAsync())
          {
            while (result.Read())
            {
              resourceIds.Add(result["Hash"].ToString());
            }
          }
        }

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT * FROM Resource WHERE Id = @resourceId";
          command.AddWithValue("resourceId", resourceId);

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
                        Channel = string.IsNullOrEmpty(decryptedChannel) ? null : this.ChannelFactory.CreateFromJson(decryptedChannel),
                        Subscription = this.SubscriptionFactory.CreateFromJson(decryptedSubscription)
                      };
          }
        }
      }
    }

    /// <inheritdoc />
    public async Task UpdateEntryAsync(ResourceEntry entry)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        long resourceId;
        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT ResourceId FROM StreamHash WHERE Hash LIKE @hash";
          command.AddWithValue("hash", $"{entry.ResourceRoots.First()}%");

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
            using (var command = connection.CreateCommand())
            {
              command.CommandText = "INSERT OR IGNORE INTO StreamHash (Hash, ResourceId) VALUES (@hash, @resourceId)";
              command.Transaction = transaction;

              command.AddWithValue("hash", hash);
              command.AddWithValue("resourceId", resourceId);

              await command.ExecuteNonQueryAsync();
            }
          }

          var encryptedChannel = this.Encryption.Encrypt(entry.ChannelToJson());
          var encryptedSubscription = this.Encryption.Encrypt(entry.SubscriptionToJson());

          using (var command = connection.CreateCommand())
          {
            command.CommandText = $"UPDATE Resource SET Channel=@encryptedChannel, Subscription=@encryptedSubscription WHERE Id=@resourceId;";
            command.Transaction = transaction;

            command.AddWithValue("encryptedChannel", encryptedChannel);
            command.AddWithValue("encryptedSubscription", encryptedSubscription);
            command.AddWithValue("resourceId", resourceId);

            await command.ExecuteNonQueryAsync();
          }

          transaction.Commit();
        }
      }
    }
  }
}