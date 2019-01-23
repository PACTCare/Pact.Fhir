namespace Pact.Fhir.Api.Controllers
{
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Presenters;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.ReadResource;

  [ApiController]
  public class FhirController : Controller
  {
    public FhirController(CreateResourceInteractor createResourceInteractor, ReadResourceInteractor readResourceInteractor)
    {
      this.CreateResourceInteractor = createResourceInteractor;
      this.ReadResourceInteractor = readResourceInteractor;
    }

    public CreateResourceInteractor CreateResourceInteractor { get; }

    public ReadResourceInteractor ReadResourceInteractor { get; }

    [Route("api/fhir/create/{type}")]
    [HttpPost]
    public async Task<IActionResult> CreateResource(string type)
    {
      string resource;
      using (var reader = new StreamReader(this.Request.Body, Encoding.UTF8))
      {
        resource = await reader.ReadToEndAsync();
      }

      var response = await this.CreateResourceInteractor.ExecuteAsync(new CreateResourceRequest { ResourceJson = resource });
      return CreateResourcePresenter.Present(response, this.Request, this.Response, type);
    }

    [Route("api/fhir/{type}/{id}")]
    [HttpGet]
    public async Task<IActionResult> ReadResource(string type, string id)
    {
      var response = await this.ReadResourceInteractor.ExecuteAsync(new ReadResourceRequest { ResourceId = id, ResourceType = type });
      return ReadResourcePresenter.Present(response, this.Response);
    }
  }
}