namespace Pact.Fhir.Api.Presenters
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;

  using Hl7.Fhir.Model;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;

  public static class CreateResourcePresenter
  {
    public static IActionResult Present(ResourceResponse response, HttpRequest httpRequest, HttpResponse httpResponse, string type)
    {
      if (response.Code == ResponseCode.Success) return PrepareRequestSuccess(response, httpRequest, httpResponse, type);

      return PresenterBase.PrepareRequestFailure(response, httpResponse, httpRequest.ContentType);
    }

    private static IActionResult PrepareRequestSuccess(ResourceResponse response, HttpRequest httpRequest, HttpResponse httpResponse, string type)
    {
      PresenterBase.SetBasicResponseAttributes(response, httpResponse, HttpStatusCode.Created);

      httpResponse.Headers.Add(
        "Location",
        $"{httpRequest.Scheme}://{httpRequest.Host.Value}/api/fhir/{type}/{response.Resource.Id}/_history/{response.Resource.VersionId}");

      var prefer = httpRequest.Headers.FirstOrDefault(h => h.Key == "Prefer");
      if (string.IsNullOrEmpty(prefer.Key)) return new EmptyFhirResult(httpRequest.ContentType);

      switch (prefer.Value.First())
      {
        case "representation":
          return new FhirResult(response.Resource, httpRequest.ContentType);
        case "OperationOutcome":
          return new FhirResult(
            new OperationOutcome
              {
                Issue = new List<OperationOutcome.IssueComponent>
                          {
                            new OperationOutcome.IssueComponent
                              {
                                Code = OperationOutcome.IssueType.Informational,
                                Severity = OperationOutcome.IssueSeverity.Information,
                                Details = new CodeableConcept { Text = "All OK" }
                              }
                          }
              },
            httpRequest.ContentType);
        default:
          return new EmptyFhirResult(httpRequest.ContentType);
      }
    }
  }
}