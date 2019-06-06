namespace Pact.Fhir.Iota.SqlLite
{
  using System.Data.SQLite;
  using System.IO;

  public static class DatabaseInitializer
  {
    public static void InitFhirDatabase(string databaseFilename)
    {
      if (File.Exists(databaseFilename))
      {
        return;
      }

      SQLiteConnection.CreateFile(databaseFilename);
      using (var connection = new SQLiteConnection($"Data Source={databaseFilename};Version=3;"))
      {
        connection.Open();

        using (var command = new SQLiteCommand(
          "CREATE TABLE Resource (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Channel TEXT NULL, Subscription TEXT NOT NULL)",
          connection))
        {
          command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(
          "CREATE TABLE StreamHash (Hash VARCHAR(81) NOT NULL PRIMARY KEY, ResourceId INTEGER NOT NULL, FOREIGN KEY (ResourceId) REFERENCES Resource(Id))",
          connection))
        {
          command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(
          "CREATE TABLE CredentialIndex (CurrentIndex INTEGER NOT NULL, Seed TEXT NOT NULL)",
          connection))
        {
          command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(
          "CREATE TABLE ResourceResolver (Reference TEXT NOT NULL PRIMARY KEY, Seed TEXT NOT NULL)",
          connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }

    public static void InitCache(string databaseFilename)
    {
      if (File.Exists(databaseFilename))
      {
        return;
      }

      SQLiteConnection.CreateFile(databaseFilename);
      using (var connection = new SQLiteConnection($"Data Source={databaseFilename};Version=3;"))
      {
        connection.Open();

        using (var command = new SQLiteCommand("CREATE TABLE TransactionCache (Hash TEXT NOT NULL PRIMARY KEY, TransactionTrytes TEXT NOT NULL)", connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }
  }
}