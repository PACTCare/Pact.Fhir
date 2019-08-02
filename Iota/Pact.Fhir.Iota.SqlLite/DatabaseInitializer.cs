namespace Pact.Fhir.Iota.SqlLite
{
  using System.Data.SQLite;
  using System.IO;

  using Pact.Fhir.Core.SqlLite.Repository;

  public static class DatabaseInitializer
  {
    public static void InitFhirDatabase(IDbConnectionSupplier connectionSupplier, string databaseFilename)
    {
      if (File.Exists(databaseFilename))
      {
        return;
      }

      SQLiteConnection.CreateFile(databaseFilename);
      using (var connection = connectionSupplier.GetConnection($"Data Source={databaseFilename};Version=3;"))
      {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText =
            "CREATE TABLE Resource (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Channel TEXT NULL, Subscription TEXT NOT NULL)";
          command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
          command.CommandText =
            "CREATE TABLE StreamHash (Hash VARCHAR(81) NOT NULL PRIMARY KEY, ResourceId INTEGER NOT NULL, FOREIGN KEY (ResourceId) REFERENCES Resource(Id))";
          command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "CREATE TABLE CredentialIndex (CurrentIndex INTEGER NOT NULL, Seed TEXT NOT NULL)";
          command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "CREATE TABLE ResourceResolver (Reference TEXT NOT NULL PRIMARY KEY, Seed TEXT NOT NULL)";
          command.ExecuteNonQuery();
        }
      }
    }

    public static void InitCache(IDbConnectionSupplier connectionSupplier, string databaseFilename)
    {
      if (File.Exists(databaseFilename))
      {
        return;
      }

      SQLiteConnection.CreateFile(databaseFilename);
      using (var connection = connectionSupplier.GetConnection($"Data Source={databaseFilename};Version=3;"))
      {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "CREATE TABLE TransactionCache (Hash TEXT NOT NULL PRIMARY KEY, TransactionTrytes TEXT NOT NULL)";
          command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "CREATE TABLE AddressCache (TransactionHash TEXT NOT NULL PRIMARY KEY, Address TEXT NOT NULL)";
          command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "CREATE TABLE BundleCache (TransactionHash TEXT NOT NULL PRIMARY KEY, Bundle TEXT NOT NULL)";
          command.ExecuteNonQuery();
        }
      }
    }
  }
}