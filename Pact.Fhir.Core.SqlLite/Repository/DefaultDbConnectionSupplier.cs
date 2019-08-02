namespace Pact.Fhir.Core.SqlLite.Repository
{
  using System.Data;
  using System.Data.Common;
  using System.Data.SQLite;

  public class DefaultDbConnectionSupplier : IDbConnectionSupplier
  {
    /// <inheritdoc />
    public DbConnection GetConnection(string databasePath)
    {
      return new SQLiteConnection(databasePath);
    }
  }
}