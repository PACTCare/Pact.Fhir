namespace Pact.Fhir.Mobile.Repository
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Mobile.Entities;

  using SQLite;

  using Task = System.Threading.Tasks.Task;

  public class SearchRepository : ISearchRepository
  {
    public SearchRepository(string databaseFilename = "resources.sqlite")
    {
      this.ConnectionString = databaseFilename;
      this.Init(databaseFilename);
    }

    private string ConnectionString { get; }

    /// <inheritdoc />
    public async Task AddResourceAsync(Resource resource)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.InsertOrReplaceAsync(MappedResource.FromResource(resource));
    }

    /// <inheritdoc />
    public async Task<List<Resource>> FindResourcesByTypeAsync(string typeName)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      var resources = await connection.QueryAsync<MappedResource>("SELECT * FROM MappedResource WHERE TypeName = ?", typeName);

      return resources.Select(r => r.ToResource()).ToList();
    }

    /// <inheritdoc />
    public async Task UpdateResourceAsync(Resource resource)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.UpdateAsync(MappedResource.FromResource(resource));
    }

    /// <inheritdoc />
    public async Task DeleteResourceAsync(string resourceId)
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.DeleteAsync(resourceId);
    }

    public async Task DeleteAllAsync()
    {
      var connection = new SQLiteAsyncConnection(this.ConnectionString);
      await connection.DeleteAllAsync<MappedResource>();
    }

    private void Init(string databaseFilename)
    {
      if (File.Exists(databaseFilename))
      {
        return;
      }

      using (var connection = new SQLiteConnection(this.ConnectionString))
      {
        connection.CreateTable<MappedResource>();
      }
    }
  }
}