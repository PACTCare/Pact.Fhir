namespace Pact.Fhir.Api.Response
{
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Mvc;

  public class EmptyFhirResult : ActionResult
  {
    public EmptyFhirResult(string contentType)
    {
      this.ContentType = contentType;
    }

    private string ContentType { get; }

    /// <inheritdoc />
    public override void ExecuteResult(ActionContext context)
    {
      context.HttpContext.Response.ContentType = this.ContentType;
      base.ExecuteResult(context);
    }

    /// <inheritdoc />
    public override async Task ExecuteResultAsync(ActionContext context)
    {
      context.HttpContext.Response.ContentType = this.ContentType;
      await base.ExecuteResultAsync(context);
    }
  }
}