namespace Pact.Fhir.Api.Controllers
{
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Cors;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Entity;
  using Pact.Fhir.Api.Presenters;
  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Services;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.GetCapabilities;
  using Pact.Fhir.Core.Usecase.ReadResource;
  using Pact.Fhir.Core.Usecase.SearchResources;
  using Pact.Fhir.Core.Usecase.ValidateResource;

  [EnableCors("Development")]
  [ApiController]
  public class FhirController : Controller
  {
    public FhirController(
      CreateResourceInteractor createResourceInteractor,
      ReadResourceInteractor readResourceInteractor,
      GetCapabilitiesInteractor capabilitiesInteractor,
      ValidateResourceInteractor validateResourceInteractor,
      SearchResourcesInteractor searchResourcesInteractor)
    {
      this.CreateResourceInteractor = createResourceInteractor;
      this.ReadResourceInteractor = readResourceInteractor;
      this.CapabilitiesInteractor = capabilitiesInteractor;
      this.ValidateResourceInteractor = validateResourceInteractor;
      this.SearchResourcesInteractor = searchResourcesInteractor;
    }

    private GetCapabilitiesInteractor CapabilitiesInteractor { get; }

    private CreateResourceInteractor CreateResourceInteractor { get; }

    private ReadResourceInteractor ReadResourceInteractor { get; }

    private ValidateResourceInteractor ValidateResourceInteractor { get; }

    private SearchResourcesInteractor SearchResourcesInteractor { get; }

    [Route("api/fhir/create/{type}")]
    [HttpPost]
    public async Task<IActionResult> CreateResourceAsync(string type)
    {
      var response = await this.CreateResourceInteractor.ExecuteAsync(
                       new CreateResourceRequest { ResourceJson = await this.Request.ReadBodyAsync() });

      return CreateResourcePresenter.Present(response, this.Request, this.Response, type);
    }

    [Route("api/fhir/metadata")]
    [HttpGet]
    public async Task<IActionResult> GetCapabilitiesAsync()
    {
      return new JsonFhirResult(await this.CapabilitiesInteractor.ExecuteAsync());
    }

    [Route("api/fhir/{type}/{id}")]
    [HttpGet]
    public async Task<IActionResult> ReadResourceAsync(string type, string id, [FromQuery(Name = "_summary")] string summaryType)
    {
      var response = await this.ReadResourceInteractor.ExecuteAsync(new ReadResourceRequest { ResourceId = id, ResourceType = type });

      return ReadResourcePresenter.Present(response, this.Response, SummaryTypeParser.Parse(summaryType));
    }

    [Route("api/fhir/{type}")]
    [HttpGet]
    public async Task<IActionResult> SearchResourcesAsync(string type)
    {
      var response = await this.SearchResourcesInteractor.ExecuteAsync(
                       new SearchResourcesRequest
                         {
                           ResourceType = type,
                           Parameters = this.Request.QueryString.HasValue ? this.Request.QueryString.Value.Substring(1) : string.Empty
                         });

      return SearchResourcesPresenter.Present(response, this.Response);
    }

    [Route("api/fhir/{type}/$validate")]
    [HttpPost]
    public async Task<IActionResult> ValidateResourceAsync()
    {
      var response = await this.ValidateResourceInteractor.ExecuteAsync(
                       new ValidateResourceRequest { ResourceJson = await this.Request.ReadBodyAsync() });

      return ValidationResultPresenter.Present(response, this.Response);
    }
  }
}