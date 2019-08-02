namespace Pact.Fhir.Core.SqlLite
{
  using System.Data;

  public static class Extensions
  {
    public static void AddWithValue(this IDbCommand command, string name, object value)
    {
      var parameter = command.CreateParameter();
      parameter.ParameterName = name;
      parameter.Value = value;

      command.Parameters.Add(parameter);
    }
  }
}