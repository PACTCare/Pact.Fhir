namespace Pact.Fhir.Api.Entity
{
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Http;

  public static class RequestExtensions
  {
    public static async Task<string> ReadBodyAsync(this HttpRequest request)
    {
      using (var reader = new StreamReader(request.Body, Encoding.UTF8))
      {
        return await reader.ReadToEndAsync();
      }
    }
  }
}