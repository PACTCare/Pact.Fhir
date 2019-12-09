namespace Pact.Fhir.Api.Controllers
{
  using System.Threading.Tasks;

  using Hl7.Fhir.Rest;

  using Microsoft.AspNetCore.Cors;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Entity;
  using Pact.Fhir.Api.Presenters;
  using Pact.Fhir.Api.Response;
  using Pact.Fhir.Core.Services;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.DeleteResource;
  using Pact.Fhir.Core.Usecase.GetCapabilities;
  using Pact.Fhir.Core.Usecase.ReadResource;
  using Pact.Fhir.Core.Usecase.ReadResourceHistory;
  using Pact.Fhir.Core.Usecase.ReadResourceVersion;
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
      SearchResourcesInteractor searchResourcesInteractor,
      ReadResourceVersionInteractor readResourceVersionInteractor,
      ReadResourceHistoryInteractor readResourceHistoryInteractor,
      DeleteResourceInteractor deleteResourceInteractor)
    {
      this.CreateResourceInteractor = createResourceInteractor;
      this.ReadResourceInteractor = readResourceInteractor;
      this.CapabilitiesInteractor = capabilitiesInteractor;
      this.ValidateResourceInteractor = validateResourceInteractor;
      this.SearchResourcesInteractor = searchResourcesInteractor;
      this.ReadResourceVersionInteractor = readResourceVersionInteractor;
      this.ReadResourceHistoryInteractor = readResourceHistoryInteractor;
      this.DeleteResourceInteractor = deleteResourceInteractor;
    }

    private GetCapabilitiesInteractor CapabilitiesInteractor { get; }

    private CreateResourceInteractor CreateResourceInteractor { get; }
    private DeleteResourceInteractor DeleteResourceInteractor { get; }

    private ReadResourceHistoryInteractor ReadResourceHistoryInteractor { get; }

    private ReadResourceInteractor ReadResourceInteractor { get; }

    private ReadResourceVersionInteractor ReadResourceVersionInteractor { get; }

    private SearchResourcesInteractor SearchResourcesInteractor { get; }

    private ValidateResourceInteractor ValidateResourceInteractor { get; }

    [Route("api/fhir/create/{type}")]
    [HttpPost]
    public async Task<IActionResult> CreateResourceAsync(string type)
    {
      var response = await this.CreateResourceInteractor.ExecuteAsync(
                       new CreateResourceRequest { ResourceJson = await this.Request.ReadBodyAsync() });

      return CreateResourcePresenter.Present(response, this.Request, this.Response, type);
    }

    [Route("api/fhir/{type}/{id}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteResourceAsync(string type, string id)
    {
      var response = await this.DeleteResourceInteractor.ExecuteAsync(new DeleteResourceRequest { ResourceId = id, ResourceType = type });
      if (response.Code == ResponseCode.Success)
      {
        return this.Ok();
      }

      return this.BadRequest();
    }

    [Route("api/fhir/metadata")]
    [HttpGet]
    public async Task<IActionResult> GetCapabilitiesAsync()
    {
      var capabilityStatement = await this.CapabilitiesInteractor.ExecuteAsync();
      return new FhirResult(capabilityStatement, this.Request.ContentType);
    }

    [Route("api/fhir/{type}/{id}")]
    [HttpGet]
    public async Task<IActionResult> ReadResourceAsync(string type, string id, [FromQuery(Name = "_summary")] string summaryType)
    {
      var response = await this.ReadResourceInteractor.ExecuteAsync(new ReadResourceRequest { ResourceId = id, ResourceType = type });

      return ReadResourcePresenter.Present(response, this.Request.ContentType, this.Response, SummaryTypeParser.Parse(summaryType));
    }

    [Route("api/fhir/{type}/{id}/_history")]
    [HttpGet]
    public async Task<IActionResult> ReadResourceHistoryAsync(string type, string id)
    {
      var response = await this.ReadResourceHistoryInteractor.ExecuteAsync(new ReadResourceHistoryRequest { ResourceType = type, ResourceId = id });

      return SearchResourcesPresenter.Present(response, this.Response, this.Request.ContentType);
    }

    [Route("api/fhir/{type}/{id}/_history/{versionId}")]
    [HttpGet]
    public async Task<IActionResult> ReadResourceVersionAsync(string type, string id, string versionId)
    {
      var response = await this.ReadResourceVersionInteractor.ExecuteAsync(
                       new ReadResourceVersionRequest { ResourceId = id, ResourceType = type, VersionId = versionId });

      return ReadResourcePresenter.Present(response, this.Request.ContentType, this.Response, SummaryType.False);
    }

    [Route("api/fhir/{type}")]
    [HttpGet]
    public async Task<IActionResult> SearchResourcesGetAsync(string type)
    {
      return await this.SearchResourcesAsync(type);
    }

    [Route("api/fhir/{type}")]
    [HttpPost]
    public async Task<IActionResult> SearchResourcesPostAsync(string type)
    {
      return await this.SearchResourcesAsync(type);
    }

    [Route("api/fhir/{type}/$validate")]
    [HttpPost]
    public async Task<IActionResult> ValidateResourceAsync()
    {
      var response = await this.ValidateResourceInteractor.ExecuteAsync(
                       new ValidateResourceRequest { ResourceJson = await this.Request.ReadBodyAsync() });

      return ValidationResultPresenter.Present(response, this.Response, this.Request.ContentType);
    }

    private async Task<IActionResult> SearchResourcesAsync(string type)
    {
      var response = await this.SearchResourcesInteractor.ExecuteAsync(
                       new SearchResourcesRequest
                         {
                           ResourceType = type,
                           Parameters = this.Request.QueryString.HasValue ? this.Request.QueryString.Value.Substring(1) : string.Empty
                         });

      return SearchResourcesPresenter.Present(response, this.Response, this.Request.ContentType);
    }
  }
}