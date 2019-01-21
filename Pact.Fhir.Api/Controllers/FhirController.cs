namespace Pact.Fhir.Api.Controllers
{
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Presenters;
  using Pact.Fhir.Core.Usecase.CreateResource;

  [ApiController]
  public class FhirController : Controller
  {
    public FhirController(CreateResourceInteractor createResourceInteractor)
    {
      this.CreateResourceInteractor = createResourceInteractor;
    }

    public CreateResourceInteractor CreateResourceInteractor { get; }

    [Route("api/fhir/create/{resourceType}")]
    [HttpPost]
    public async Task<IActionResult> CreateResource(string resourceType)
    {
      string resource;
      using (var reader = new StreamReader(this.Request.Body, Encoding.UTF8))
      {
        resource = await reader.ReadToEndAsync();
      }

      var response = await this.CreateResourceInteractor.ExecuteAsync(new CreateResourceRequest { ResourceJson = resource, ResourceType = resourceType });
      return CreateResourcePresenter.Present(response, this.Request, this.Response);
    }
  }
}