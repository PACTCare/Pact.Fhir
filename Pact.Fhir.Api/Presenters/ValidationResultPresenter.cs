namespace Pact.Fhir.Api.Presenters
{
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.ValidateResource;

  public static class ValidationResultPresenter
  {
    public static IActionResult Present(ValidateResourceResponse response, HttpResponse httpResponse, string contentType)
    {
      if (response.Code == ResponseCode.Success)
      {
        if (response.ValidationResult.Count == 0)
        {
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
            contentType);
        }

        return new FhirResult(
          new OperationOutcome
            {
              Issue = response.ValidationResult.Select(
                r => new OperationOutcome.IssueComponent
                       {
                         Severity = OperationOutcome.IssueSeverity.Error, Details = new CodeableConcept { Text = r.ToString() }
                       }).ToList()
            },
          contentType);
      }

      return PresenterBase.PrepareRequestFailure(response, httpResponse, contentType);
    }
  }
}