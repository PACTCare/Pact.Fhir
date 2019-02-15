namespace Pact.Fhir.Api.Presenters
{
  using System.Collections.Generic;
  using System.Net;

  using Hl7.Fhir.Model;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;

  public static class PresenterBase
  {
    internal static IActionResult PrepareRequestFailure(UsecaseResponse response, HttpResponse httpResponse)
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
                Severity = OperationOutcome.IssueSeverity.Error,
                Details = new CodeableConcept { Text = response.ExceptionMessage }
              });
          break;
        case ResponseCode.UnsupportedResource:
          httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
          outcome.Issue.Add(
            new OperationOutcome.IssueComponent
              {
                Code = OperationOutcome.IssueType.NotSupported,
                Severity = OperationOutcome.IssueSeverity.Error,
                Details = new CodeableConcept { Text = response.ExceptionMessage }
              });
          break;
        case ResponseCode.ResourceNotFound:
          httpResponse.StatusCode = (int)HttpStatusCode.NotFound;
          outcome.Issue.Add(
            new OperationOutcome.IssueComponent
              {
                Code = OperationOutcome.IssueType.NotFound,
                Severity = OperationOutcome.IssueSeverity.Error,
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

    internal static void SetBasicResponseAttributes(ResourceResponse response, HttpResponse httpResponse, HttpStatusCode statusCode)
    {
      httpResponse.StatusCode = (int)statusCode;
      httpResponse.Headers.Add("ETag", $"W/\"{response.Resource.VersionId}\"");

      if (response.Resource.Meta.LastUpdated.HasValue)
      {
        httpResponse.Headers.Add("Last-Modified", response.Resource.Meta.LastUpdated.Value.ToString("O"));
      }
    }
  }
}