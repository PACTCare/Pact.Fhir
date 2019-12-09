namespace Pact.Fhir.Api.Presenters
{
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;

  public static class SearchResourcesPresenter
  {
    public static IActionResult Present(ResourceResponse response, HttpResponse httpResponse, string contentType)
    {
      return response.Code == ResponseCode.Success
               ? new FhirResult(response.Resource, contentType)
               : PresenterBase.PrepareRequestFailure(response, httpResponse, contentType);
    }
  }
}