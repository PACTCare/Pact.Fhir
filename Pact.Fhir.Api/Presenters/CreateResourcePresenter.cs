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
    public static IActionResult Present(ResourceUsecaseResponse response, HttpRequest httpRequest, HttpResponse httpResponse, string type)
    {
      if (response.Code == ResponseCode.Success)
      {
        return PrepareRequestSuccess(response, httpRequest, httpResponse, type);
      }

      return PresenterBase.PrepareRequestFailure(response, httpResponse);
    }

    private static IActionResult PrepareRequestSuccess(ResourceUsecaseResponse response, HttpRequest httpRequest, HttpResponse httpResponse, string type)
    {
      PresenterBase.SetBasicResponseAttributes(response, httpResponse, HttpStatusCode.Created);

      httpResponse.Headers.Add(
        "Location",
        $"{httpRequest.Scheme}://{httpRequest.Host.Value}/api/fhir/{type}/{response.Resource.Id}/_history/{response.Resource.VersionId}");

      var prefer = httpRequest.Headers.FirstOrDefault(h => h.Key == "Prefer");
      if (string.IsNullOrEmpty(prefer.Key))
      {
        return new EmptyJsonFhirResult();
      }

      switch (prefer.Value.First())
      {
        case "representation":
          return new JsonFhirResult(response.Resource);
        case "OperationOutcome":
          return new JsonFhirResult(
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
              });
        default:
          return new EmptyJsonFhirResult();
      }
    }
  }
}