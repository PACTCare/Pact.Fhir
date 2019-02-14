namespace Pact.Fhir.Api.Presenters
{
  using System.Net;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;

  public static class ReadResourcePresenter
  {
    public static IActionResult Present(UsecaseResponse response, HttpResponse httpResponse, string summaryType)
    {
      if (response.Code == ResponseCode.Success)
      {
        PresenterBase.SetBasicResponseAttributes(response, httpResponse, HttpStatusCode.OK);

        switch (summaryType)
        {
          case "true":
          case "text":
          case "data":
          case "count":
            default:
              return new JsonFhirResult(response.Resource);
        }
      }

      return PresenterBase.PrepareRequestFailure(response, httpResponse);
    }
  }
}