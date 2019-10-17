namespace Pact.Fhir.Core.SqlLite.Repository
{
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SQLite;
  using System.IO;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Repository;

  using Task = System.Threading.Tasks.Task;

  public class SqlLiteSearchRepository : ISearchRepository
  {
    public SqlLiteSearchRepository(
      FhirJsonParser jsonParser,
      IDbConnectionSupplier connectionSupplier = null,
      string databaseFilename = "resources.sqlite")
    {
      this.JsonParser = jsonParser;
      this.ConnectionSupplier = connectionSupplier ?? new DefaultDbConnectionSupplier();
      this.ConnectionString = $"Data Source={databaseFilename};Version=3;";
      this.Init(databaseFilename);
    }

    private string ConnectionString { get; }

    private IDbConnectionSupplier ConnectionSupplier { get; }

    private FhirJsonParser JsonParser { get; }

    /// <inheritdoc />
    public async Task AddResourceAsync(Resource resource)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "INSERT OR IGNORE INTO Resources (Id, VersionId, TypeName, Payload) VALUES (@Id, @VersionId, @TypeName, @Payload)";

          command.AddWithValue("Id", resource.Id);
          command.AddWithValue("VersionId", resource.VersionId);
          command.AddWithValue("TypeName", resource.TypeName);
          command.AddWithValue("Payload", resource.ToJson());

          await command.ExecuteNonQueryAsync();
        }
      }
    }

    /// <inheritdoc />
    public async Task<List<Resource>> FindResourcesByTypeAsync(string typeName)
    {
      var resources = new List<Resource>();

      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT Payload FROM Resources WHERE TypeName=@typeName";

          command.AddWithValue("TypeName", typeName);

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
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText =
            "INSERT INTO Resources (Id, VersionId, TypeName, Payload) VALUES (@Id, @VersionId, @TypeName, @Payload) ON CONFLICT (Id) DO UPDATE SET VersionId=@VersionId, TypeName=@TypeName, Payload=@Payload";

          command.AddWithValue("Id", resource.Id);
          command.AddWithValue("VersionId", resource.VersionId);
          command.AddWithValue("TypeName", resource.TypeName);
          command.AddWithValue("Payload", resource.ToJson());

          await command.ExecuteNonQueryAsync();
        }
      }
    }

    /// <inheritdoc />
    public async Task DeleteResourceAsync(string resourceId)
    {
      using (var connection = this.ConnectionSupplier.GetConnection(this.ConnectionString))
      {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "DELETE FROM Resources WHERE Id=@Id";

          command.AddWithValue("Id", resourceId);
          await command.ExecuteNonQueryAsync();
        }
      }
    }

    private void Init(string databaseFilename)
    {
      if (File.Exists(databaseFilename))
      {
        return;
      }

      SQLiteConnection.CreateFile(databaseFilename);
      using (var connection = this.ConnectionSupplier.GetConnection($"Data Source={databaseFilename};Version=3;"))
      {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText =
            "CREATE TABLE Resources (Id TEXT NOT NULL PRIMARY KEY, VersionId TEXT NOT NULL, TypeName TEXT NOT NULL, Payload TEXT NOT NULL)";

          command.ExecuteNonQuery();
        }
      }
    }
  }
}