namespace Pact.Fhir.Iota.SqlLite
{
  using System.Data.SQLite;
  using System.IO;

  public static class DatabaseInitializer
  {
    public static void Init(string databaseFilename)
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
          "CREATE TABLE CredentialIndex (CurrentIndex INTEGER NOT NULL)",
          connection))
        {
          command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(
          "INSERT INTO CredentialIndex (CurrentIndex) VALUES (-1)",
          connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }
  }
}