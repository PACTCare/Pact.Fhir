namespace Pact.Fhir.Api.Presenters
{
  using System.Net;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.CreateResource;

  public static class CreateResourcePresenter
  {
    public static IActionResult Present(CreateResourceResponse response, HttpResponse httpResponse)
    {
      switch (response.Code)
      {
        case ResponseCode.Success:
          httpResponse.StatusCode = (int)HttpStatusCode.Created;

          // TODO: Change location header to absolute path
          httpResponse.Headers.Add("Location", $"/api/fhir/Patient/{response.LogicalId}/_history/{response.VersionId}");
          httpResponse.Headers.Add("ETag", response.VersionId);

          if (response.LastModified.HasValue)
          {
            httpResponse.Headers.Add("Last-Modified", response.LastModified.Value.ToString("O"));
          }
          break;
        case ResponseCode.UnprocessableEntity:
          httpResponse.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
          break;
        case ResponseCode.ResourceNotFound:
        case ResponseCode.UnsupportedResource:
          httpResponse.StatusCode = (int)HttpStatusCode.NotFound;
          break;
        default:
          httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
          break;
      }

      return new JsonResult(response.Code);
    }
  }
}