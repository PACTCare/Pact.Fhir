namespace Pact.Fhir.Api.Response
{
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Mvc;

  public class EmptyJsonFhirResult : ActionResult
  {
    /// <inheritdoc />
    public override void ExecuteResult(ActionContext context)
    {
      context.HttpContext.Response.ContentType = "application/fhir+json";
      base.ExecuteResult(context);
    }

    /// <inheritdoc />
    public override async Task ExecuteResultAsync(ActionContext context)
    {
      context.HttpContext.Response.ContentType = "application/fhir+json";
      await base.ExecuteResultAsync(context);
    }
  }
}