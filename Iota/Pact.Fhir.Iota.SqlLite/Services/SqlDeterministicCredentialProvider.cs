namespace Pact.Fhir.Iota.SqlLite.Services
{
  using System.Data.SQLite;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;
  using Tangle.Net.Repository;

  public class SqlDeterministicCredentialProvider : DeterministicCredentialProvider
  {
    /// <inheritdoc />
    public SqlDeterministicCredentialProvider(
      Seed masterSeed,
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator,
      IIotaRepository repository,
      string databaseFilename = "iotafhir.sqlite")
      : base(masterSeed, resourceTracker, signingHelper, addressGenerator, repository)
    {
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      DatabaseInitializer.Init(databaseFilename);
    }

    private string ConnectionString { get; }

    /// <inheritdoc />
    protected override async Task<int> GetCurrentSubSeedIndexAsync()
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = new SQLiteCommand("SELECT CurrentIndex FROM CredentialIndex", connection))
        {
          return int.Parse((await command.ExecuteScalarAsync()).ToString());
        }
      }
    }

    /// <inheritdoc />
    protected override async Task SetCurrentSubSeedIndexAsync(int index)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = new SQLiteCommand($"UPDATE CredentialIndex SET CurrentIndex={index}", connection))
        {
          await command.ExecuteNonQueryAsync();
        }
      }
    }
  }
}