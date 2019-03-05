namespace Pact.Fhir.Api.Controllers
{
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Entity;
  using Pact.Fhir.Api.Presenters;
  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Services;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.GetCapabilities;
  using Pact.Fhir.Core.Usecase.ReadResource;

  [ApiController]
  public class FhirController : Controller
  {
    public FhirController(CreateResourceInteractor createResourceInteractor, ReadResourceInteractor readResourceInteractor, GetCapabilitiesInteractor capabilitiesInteractor)
    {
      this.CreateResourceInteractor = createResourceInteractor;
      this.ReadResourceInteractor = readResourceInteractor;
      this.CapabilitiesInteractor = capabilitiesInteractor;
    }

    private CreateResourceInteractor CreateResourceInteractor { get; }

    private ReadResourceInteractor ReadResourceInteractor { get; }

    private GetCapabilitiesInteractor CapabilitiesInteractor { get; }

    [Route("api/fhir/create/{type}")]
    [HttpPost]
    public async Task<IActionResult> CreateResourceAsync(string type)
    {
      var response = await this.CreateResourceInteractor.ExecuteAsync(
                       new CreateResourceRequest { ResourceJson = await this.Request.ReadBodyAsync() });

      return CreateResourcePresenter.Present(response, this.Request, this.Response, type);
    }

    [Route("api/fhir/{type}/{id}")]
    [HttpGet]
    public async Task<IActionResult> ReadResourceAsync(string type, string id, [FromQuery(Name = "_summary")] string summaryType)
    {
      var response = await this.ReadResourceInteractor.ExecuteAsync(new ReadResourceRequest { ResourceId = id, ResourceType = type });
      return ReadResourcePresenter.Present(response, this.Response, SummaryTypeParser.Parse(summaryType));
    }

    [Route("api/fhir/metadata")]
    [HttpGet]
    public async Task<IActionResult> GetCapabilitiesAsync()
    {
      return new JsonFhirResult(await this.CapabilitiesInteractor.ExecuteAsync());
    }
  }
}