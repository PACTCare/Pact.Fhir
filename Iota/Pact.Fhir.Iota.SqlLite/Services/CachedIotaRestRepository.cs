namespace Pact.Fhir.Iota.SqlLite.Services
{
  using System;
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.Linq;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.SqlLite;
  using Pact.Fhir.Core.SqlLite.Repository;

  using RestSharp;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.ProofOfWork;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;
  using Tangle.Net.Repository.DataTransfer;

  public class CachedIotaRestRepository : RestIotaRepository
  {
    private IDbConnectionSupplier ConnectionSupplier { get; }

    /// <inheritdoc />
    public CachedIotaRestRepository(
      IRestClient client,
      IPoWService powService = null,
      string username = null,
      string password = null,
      IDbConnectionSupplier connectionSupplier = null,
      string databaseFilename = "iotacache.sqlite")
      : base(client, powService, username, password)
    {
      this.ConnectionSupplier = connectionSupplier ?? new DefaultDbConnectionSupplier();
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      DatabaseInitializer.InitCache(this.ConnectionSupplier, databaseFilename);
    }

    /// <inheritdoc />
    public CachedIotaRestRepository(
      IIotaClient client,
      IPoWService powService,
      IAddressGenerator addressGenerator = null,
      IDbConnectionSupplier connectionSupplier = null,
      string databaseFilename = "iotacache.sqlite")
      : base(client, powService, addressGenerator)
    {
      this.ConnectionSupplier = connectionSupplier ?? new DefaultDbConnectionSupplier();
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      DatabaseInitializer.InitCache(this.ConnectionSupplier, databaseFilename);
    }

    private string ConnectionString { get; }

    /// <inheritdoc />
    public override async Task<List<TransactionTrytes>> GetTrytesAsync(List<Hash> hashes)
    {
      var transactionTrytes = new List<string>();
      var parameters = hashes.Select(h => $"'{h.Value}'").Distinct().ToList();

      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = $"SELECT * FROM TransactionCache WHERE Hash IN ({string.Join(", ", parameters)})";

          var result = await command.ExecuteReaderAsync();
          while (result.Read())
          {
            transactionTrytes.Add(result["TransactionTrytes"] as string);
          }
        }
      }

      if (transactionTrytes.Count == parameters.Count)
      {
        return transactionTrytes.Select(t => new TransactionTrytes(t)).ToList();
      }

      var transactions = await base.GetTrytesAsync(hashes);
      await this.StoreTransactionsInCache(transactions);

      return transactions;
    }

    /// <inheritdoc />
    public override async Task<TransactionHashList> FindTransactionsByAddressesAsync(IEnumerable<Address> addresses)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        var parameters = addresses.Select(a => $"'{a.Value}'").Distinct().ToList();

        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = $"SELECT * FROM AddressCache WHERE Address IN ({string.Join(", ", parameters)})";
          var result = await command.ExecuteReaderAsync();
          var transactionHashes = new List<string>();

          while (result.Read())
          {
            transactionHashes.Add(result["TransactionHash"] as string);
          }

          if (transactionHashes.Count > 0)
          {
            return new TransactionHashList { Hashes = transactionHashes.Select(t => new Hash(t)).ToList() };
          }
        }
      }

      return await base.FindTransactionsByAddressesAsync(addresses);
    }

    /// <inheritdoc />
    public override async Task<TransactionHashList> FindTransactionsByBundlesAsync(IEnumerable<Hash> bundleHashes)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        var parameters = bundleHashes.Select(a => $"'{a.Value}'").Distinct().ToList();

        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = $"SELECT * FROM BundleCache WHERE Bundle IN ({string.Join(", ", parameters)})";

          var transactionHashes = new List<string>();
          var result = await command.ExecuteReaderAsync();
          while (result.Read())
          {
            transactionHashes.Add(result["TransactionHash"] as string);
          }

          if (transactionHashes.Count > 0)
          {
            return new TransactionHashList { Hashes = transactionHashes.Select(t => new Hash(t)).ToList() };
          }
        }
      }

      return await base.FindTransactionsByBundlesAsync(bundleHashes);
    }

    /// <inheritdoc />
    public override async Task StoreTransactionsAsync(IEnumerable<TransactionTrytes> transactions)
    {
      await this.StoreTransactionsInCache(transactions);
      await base.StoreTransactionsAsync(transactions);
    }

    private async Task StoreTransactionsInCache(IEnumerable<TransactionTrytes> transactions)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var sqlTransaction = connection.BeginTransaction())
        {
          foreach (var transaction in transactions)
          {
            var parsedTransaction = Transaction.FromTrytes(transaction);

            using (var command = connection.CreateCommand())
            {
              command.CommandText = "INSERT OR IGNORE INTO TransactionCache (Hash, TransactionTrytes) VALUES (@hash, @transactionTrytes)";

              command.AddWithValue("hash", parsedTransaction.Hash.Value);
              command.AddWithValue("transactionTrytes", transaction.Value);

              await command.ExecuteNonQueryAsync();
            }

            using (var command = connection.CreateCommand())
            {
              command.CommandText = "INSERT OR IGNORE INTO AddressCache (TransactionHash, Address) VALUES (@transactionHash, @address)";

              command.AddWithValue("transactionHash", parsedTransaction.Hash.Value);
              command.AddWithValue("address", parsedTransaction.Address.Value);

              await command.ExecuteNonQueryAsync();
            }

            using (var command = connection.CreateCommand())
            {
              command.CommandText = "INSERT OR IGNORE INTO BundleCache (TransactionHash, Bundle) VALUES (@transactionHash, @bundle)";

              command.AddWithValue("transactionHash", parsedTransaction.Hash.Value);
              command.AddWithValue("bundle", parsedTransaction.BundleHash.Value);

              await command.ExecuteNonQueryAsync();
            }
          }

          sqlTransaction.Commit();
        }
      }
    }
  }
}