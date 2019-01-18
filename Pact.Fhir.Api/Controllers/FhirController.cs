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
      string resourceJson;
      using (var reader = new StreamReader(this.Request.Body, Encoding.UTF8))
      {
        resourceJson = await reader.ReadToEndAsync();
      }

      var response = await this.CreateResourceInteractor.ExecuteAsync(new CreateResourceRequest { ResourceJson = resourceJson, ResourceType = resourceType });
      return CreateResourcePresenter.Present(response, this.Response);
    }
  }
}