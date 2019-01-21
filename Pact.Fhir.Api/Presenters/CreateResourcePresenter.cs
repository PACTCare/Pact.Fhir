﻿namespace Pact.Fhir.Api.Presenters
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;

  using Hl7.Fhir.Model;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.CreateResource;

  public static class CreateResourcePresenter
  {
    public static IActionResult Present(CreateResourceResponse response, HttpRequest httpRequest, HttpResponse httpResponse)
    {
      if (response.Code == ResponseCode.Success)
      {
        return PrepareRequestSuccess(response, httpRequest, httpResponse);
      }

      return PrepareRequestFailure(response, httpResponse);
    }

    private static IActionResult PrepareRequestSuccess(CreateResourceResponse response, HttpRequest httpRequest, HttpResponse httpResponse)
    {
      httpResponse.StatusCode = (int)HttpStatusCode.Created;

      httpResponse.Headers.Add(
        "Location",
        $"{httpRequest.Scheme}://{httpRequest.Host.Value}/api/fhir/Patient/{response.Resource.Id}/_history/{response.Resource.VersionId}");
      httpResponse.Headers.Add("ETag", response.Resource.VersionId);

      if (response.Resource.Meta.LastUpdated.HasValue)
      {
        httpResponse.Headers.Add("Last-Modified", response.Resource.Meta.LastUpdated.Value.ToString("O"));
      }

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

    private static IActionResult PrepareRequestFailure(UsecaseResponse response, HttpResponse httpResponse)
    {
      var outcome = new OperationOutcome { Issue = new List<OperationOutcome.IssueComponent>() };
      switch (response.Code)
      {
        case ResponseCode.UnprocessableEntity:
          httpResponse.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
          outcome.Issue.Add(
            new OperationOutcome.IssueComponent
              {
                Code = OperationOutcome.IssueType.Structure,
                Severity = OperationOutcome.IssueSeverity.Fatal,
                Details = new CodeableConcept { Text = response.ExceptionMessage }
              });
          break;
        case ResponseCode.UnsupportedResource:
          httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
          outcome.Issue.Add(
            new OperationOutcome.IssueComponent
              {
                Code = OperationOutcome.IssueType.NotSupported,
                Severity = OperationOutcome.IssueSeverity.Fatal,
                Details = new CodeableConcept { Text = response.ExceptionMessage }
              });
          break;
        default:
          httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
          outcome.Issue.Add(
            new OperationOutcome.IssueComponent
              {
                Code = OperationOutcome.IssueType.Exception,
                Severity = OperationOutcome.IssueSeverity.Fatal,
                Details = new CodeableConcept { Text = response.ExceptionMessage }
              });
          break;
      }

      return new JsonFhirResult(outcome);
    }
  }
}