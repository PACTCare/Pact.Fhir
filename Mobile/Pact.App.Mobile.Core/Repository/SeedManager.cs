namespace Pact.Fhir.Mobile.Repository
{
  using System.IO;
  using System.Security.Cryptography;
  using System.Text;
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;
  using Pact.Fhir.Mobile.Entities;

  using SQLite;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;
  using Tangle.Net.Repository;

  public class SeedManager : DeterministicSeedManager
  {
    private IEncryption Encryption { get; }

    /// <inheritdoc />
    public SeedManager(
      IResourceTracker resourceTracker,
      ISigningHelper signingHelper,
      IAddressGenerator addressGenerator,
      IIotaRepository repository,
      IEncryption encryption,
      string databaseFilename = "iotafhir.sqlite")
      : base(resourceTracker, signingHelper, addressGenerator, repository)
    {
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
        connection.CreateTable<ReferenceEntry>();
        connection.CreateTable<SeedIndexEntry>();
      }
    }

    private string ConnectionString { get; }

    /// <inheritdoc />
    public override async Task AddReferenceAsync(string reference, Seed seed)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.InsertOrReplaceAsync(new ReferenceEntry { Reference = reference, Seed = this.Encryption.Encrypt(seed.Value) });
    }

    /// <inheritdoc />
    public override async Task DeleteReferenceAsync(string reference)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.DeleteAsync<ReferenceEntry>(reference);
    }

    /// <inheritdoc />
    public override async Task<Seed> ResolveReferenceAsync(string reference = null)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      var referenceEntry = await connection.FindAsync<ReferenceEntry>(reference);

      return new Seed(this.Encryption.Decrypt(referenceEntry.Seed));
    }

    /// <inheritdoc />
    protected override async Task<int> GetCurrentSubSeedIndexAsync(Seed seed)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      var index = await connection.FindAsync<SeedIndexEntry>(ComputeSeedHash(seed));

      if (index == null)
      {
        return 0;
      }

      return index.Index;
    }

    /// <inheritdoc />
    protected override async Task SetCurrentSubSeedIndexAsync(Seed seed, int index)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.InsertOrReplaceAsync(new SeedIndexEntry { Index = index, SeedHash = ComputeSeedHash(seed) });
    }

    public async Task DeleteAllAsync()
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.DeleteAllAsync<SeedIndexEntry>();
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