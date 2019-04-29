namespace Pact.Fhir.Iota.SqlLite.Services
{
  using System;
  using System.Data.SQLite;

  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;

  using Tangle.Net.Entity;

  public class SqlLiteReferenceResolver : IReferenceResolver
  {
    public SqlLiteReferenceResolver(IEncryption encryption, string databaseFilename = "iotafhir.sqlite")
    {
      this.Encryption = encryption;
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";

      DatabaseInitializer.Init(databaseFilename);
    }

    private string ConnectionString { get; }

    private IEncryption Encryption { get; }

    /// <inheritdoc />
    public void AddReference(string reference, Seed seed)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.Open();

        using (var command = new SQLiteCommand("INSERT OR IGNORE INTO ResourceResolver (Reference, Seed) VALUES (@reference, @encryptedSeed)", connection))
        {
          command.Parameters.AddWithValue("reference", reference);
          command.Parameters.AddWithValue("encryptedSeed", this.Encryption.Encrypt(seed.Value));

          command.ExecuteNonQuery();
        }
      }
    }

    /// <inheritdoc />
    public Seed Resolve(string reference)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.Open();

        using (var command = new SQLiteCommand("SELECT Seed FROM ResourceResolver WHERE Reference=@reference", connection))
        {
          command.Parameters.AddWithValue("reference", reference);

          var result = command.ExecuteScalar();
          if (result == null)
          {
            throw new ArgumentException("Unkown reference");
          }

          return new Seed(this.Encryption.Decrypt(result as string));
        }
      }
    }
  }
}