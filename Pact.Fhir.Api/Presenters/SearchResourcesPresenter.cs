namespace Pact.Fhir.Api.Presenters
{
  using System.Net;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;

  public class SearchResourcesPresenter
  {
    public static IActionResult Present(ResourceResponse response, HttpResponse httpResponse)
    {
      if (response.Code == ResponseCode.Success)
      {
        return new JsonFhirResult(response.Resource);
      }

      return PresenterBase.PrepareRequestFailure(response, httpResponse);
    }
  }
}