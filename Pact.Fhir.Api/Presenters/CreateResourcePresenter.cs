namespace Pact.Fhir.Api.Presenters
{
  using System.Net;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.CreateResource;

  public static class CreateResourcePresenter
  {
    public static IActionResult Present(CreateResourceResponse response, HttpRequest httpRequest, HttpResponse httpResponse)
    {
      switch (response.Code)
      {
        case ResponseCode.Success:
          httpResponse.StatusCode = (int)HttpStatusCode.Created;

          httpResponse.Headers.Add(
            "Location",
            $"{httpRequest.Scheme}://{httpRequest.Host.Value}/api/fhir/Patient/{response.Resource.Id}/_history/{response.Resource.VersionId}");
          httpResponse.Headers.Add("ETag", response.Resource.VersionId);

          if (response.Resource.Meta.LastUpdated.HasValue)
          {
            httpResponse.Headers.Add("Last-Modified", response.Resource.Meta.LastUpdated.Value.ToString("O"));
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