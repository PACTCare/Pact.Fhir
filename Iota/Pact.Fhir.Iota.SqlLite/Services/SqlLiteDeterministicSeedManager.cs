namespace Pact.Fhir.Iota.SqlLite.Services
{
  using System;
  using System.Data.SQLite;
  using System.Security.Cryptography;
  using System.Text;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.SqlLite;
  using Pact.Fhir.Core.SqlLite.Repository;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;
  using Tangle.Net.Repository;

  public class SqlLiteDeterministicSeedManager : DeterministicSeedManager
  {
    private IEncryption Encryption { get; }
    private IDbConnectionSupplier ConnectionSupplier { get; }

    /// <inheritdoc />
    public SqlLiteDeterministicSeedManager(
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator,
      IIotaRepository repository,
      IEncryption encryption,
      IDbConnectionSupplier connectionSupplier = null,
      string databaseFilename = "iotafhir.sqlite")
      : base(resourceTracker, signingHelper, addressGenerator, repository)
    {
      this.Encryption = encryption;
      this.ConnectionSupplier = connectionSupplier ?? new DefaultDbConnectionSupplier();
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      DatabaseInitializer.InitFhirDatabase(this.ConnectionSupplier, databaseFilename);
    }

    private string ConnectionString { get; }

    /// <inheritdoc />
    public override async Task AddReferenceAsync(string reference, Seed seed)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "INSERT OR IGNORE INTO ResourceResolver (Reference, Seed) VALUES (@reference, @encryptedSeed)";

          command.AddWithValue("reference", reference);
          command.AddWithValue("encryptedSeed", this.Encryption.Encrypt(seed.Value));

          await command.ExecuteNonQueryAsync();
        }
      }
    }

    /// <inheritdoc />
    public override async Task DeleteReferenceAsync(string reference)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "DELETE FROM ResourceResolver WHERE Reference = @reference";
          command.AddWithValue("reference", reference);

          await command.ExecuteNonQueryAsync();
        }
      }
    }

    /// <inheritdoc />
    public override async Task<Seed> ResolveReferenceAsync(string reference = null)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT Seed FROM ResourceResolver WHERE Reference=@reference";
          command.AddWithValue("reference", reference);

          var result = await command.ExecuteScalarAsync();
          if (result == null)
          {
            throw new ArgumentException("Unkown reference");
          }

          return new Seed(this.Encryption.Decrypt(result as string));
        }
      }
    }

    /// <inheritdoc />
    protected override async Task<int> GetCurrentSubSeedIndexAsync(Seed seed)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT CurrentIndex FROM CredentialIndex WHERE Seed=@seed";

          var seedHash = ComputeSeedHash(seed);
          command.AddWithValue("seed", seedHash);

          var result = await command.ExecuteScalarAsync();
          if (result != null)
          {
            return int.Parse(result.ToString());
          }

          using (var innerCommand = connection.CreateCommand())
          {
            innerCommand.CommandText = "INSERT INTO CredentialIndex (CurrentIndex, Seed) VALUES (0, @seed)";

            innerCommand.AddWithValue("seed", seedHash);
            await innerCommand.ExecuteNonQueryAsync();
          }

          return 0;
        }
      }
    }

    /// <inheritdoc />
    protected override async Task SetCurrentSubSeedIndexAsync(Seed seed, int index)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "UPDATE CredentialIndex SET CurrentIndex=@index WHERE Seed=@seed";

          command.AddWithValue("index", index);
          command.AddWithValue("seed", ComputeSeedHash(seed));

          await command.ExecuteNonQueryAsync();
        }
      }
    }

    private static string ComputeSeedHash(TryteString seed)
    {
      using (var alg = SHA256.Create())
      {
        var hash = alg.ComputeHash(Encoding.UTF8.GetBytes(seed.Value));
        return Encoding.UTF8.GetString(hash);
      }
    }
  }
}