namespace Pact.Fhir.Core.SqlLite.Repository
{
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.IO;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Repository;

  using Task = System.Threading.Tasks.Task;

  public class SqlLiteSearchRepository : ISearchRepository
  {
    public SqlLiteSearchRepository(FhirJsonParser jsonParser, string databaseFilename = "resources.sqlite")
    {
      this.JsonParser = jsonParser;
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";
      Init(databaseFilename);
    }

    private string ConnectionString { get; }

    private FhirJsonParser JsonParser { get; }

    /// <inheritdoc />
    public async Task AddResourceAsync(Resource resource)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.Open();

        using (var command = new SQLiteCommand(
          "INSERT OR IGNORE INTO Resources (Id, VersionId, TypeName, Payload) VALUES (@Id, @VersionId, @TypeName, @Payload)",
          connection))
        {
          command.Parameters.AddWithValue("Id", resource.Id);
          command.Parameters.AddWithValue("VersionId", resource.VersionId);
          command.Parameters.AddWithValue("TypeName", resource.TypeName);
          command.Parameters.AddWithValue("Payload", resource.ToJson());

          await command.ExecuteNonQueryAsync();
        }
      }
    }

    /// <inheritdoc />
    public async Task<List<Resource>> FindResourcesByTypeAsync(string typeName)
    {
      var resources = new List<Resource>();

      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.Open();

        using (var command = new SQLiteCommand("SELECT Payload FROM Resources WHERE TypeName=@typeName", connection))
        {
          command.Parameters.AddWithValue("TypeName", typeName);

          var result = await command.ExecuteReaderAsync();
          while (result.Read())
          {
            resources.Add(this.JsonParser.Parse<Resource>(result["Payload"].ToString()));
          }
        }
      }

      return resources;
    }

    /// <inheritdoc />
    public async Task UpdateResourceAsync(Resource resource)
    {
      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.Open();

        using (var command = new SQLiteCommand(
          "INSERT INTO Resources (Id, VersionId, TypeName, Payload) VALUES (@Id, @VersionId, @TypeName, @Payload) ON CONFLICT (Id) DO UPDATE SET VersionId=@VersionId, TypeName=@TypeName, Payload=@Payload",
          connection))
        {
          command.Parameters.AddWithValue("Id", resource.Id);
          command.Parameters.AddWithValue("VersionId", resource.VersionId);
          command.Parameters.AddWithValue("TypeName", resource.TypeName);
          command.Parameters.AddWithValue("Payload", resource.ToJson());

          await command.ExecuteNonQueryAsync();
        }
      }
    }

    private static void Init(string databaseFilename)
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
          "CREATE TABLE Resources (Id TEXT NOT NULL PRIMARY KEY, VersionId TEXT NOT NULL, TypeName TEXT NOT NULL, Payload TEXT NOT NULL)",
          connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }
  }
}