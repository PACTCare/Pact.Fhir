namespace Pact.Fhir.Core.SqlLite.Repository
{
  using System.Data;
  using System.Data.Common;
  using System.Data.SQLite;

  public interface IDbConnectionSupplier
  {
    DbConnection GetConnection(string databasePath);
  }
}