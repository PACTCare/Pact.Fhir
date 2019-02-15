namespace Pact.Fhir.Api.Presenters
{
  using System.Net;

  using Hl7.Fhir.Rest;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;

  public static class ReadResourcePresenter
  {
    public static IActionResult Present(UsecaseResponse response, HttpResponse httpResponse, SummaryType summaryType)
    {
      if (response.Code == ResponseCode.Success)
      {
        PresenterBase.SetBasicResponseAttributes(response, httpResponse, HttpStatusCode.OK);
        return new JsonFhirResult(response.Resource, summaryType);
      }

      return PresenterBase.PrepareRequestFailure(response, httpResponse);
    }
  }
}