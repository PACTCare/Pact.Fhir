using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pact.Fhir.Api.Presenters
{
  using System.Net;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.ReadResource;

  public static class ReadResourcePresenter
  {
    public static IActionResult Present(ResourceUsecaseResponse response, HttpResponse httpResponse)
    {
      if (response.Code == ResponseCode.Success)
      {
        PresenterBase.SetBasicResponseAttributes(response, httpResponse, HttpStatusCode.OK);
        return new JsonFhirResult(response.Resource);
      }

      return PresenterBase.PrepareRequestFailure(response, httpResponse);
    }
  }
}
