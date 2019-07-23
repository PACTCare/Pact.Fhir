namespace Pact.Fhir.Api.Controllers
{
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Cors;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Models;
  using Pact.Fhir.Iota.Services;

  [EnableCors("Development")]
  [ApiController]
  public class IotaController : Controller
  {
    public IotaController(ResourceImporter resourceImporter, ISeedManager seedManager)
    {
      this.ResourceImporter = resourceImporter;
      this.SeedManager = seedManager;
    }

    private ResourceImporter ResourceImporter { get; }
    private ISeedManager SeedManager { get; }

    [Route("api/iota/import")]
    [HttpPost]
    public async Task<IActionResult> ImportResourceAccessAsync([FromBody] MamKeyPair mamKeyPair)
    {
      await this.ResourceImporter.ImportResourceAccessAsync(mamKeyPair.Root, mamKeyPair.ChannelKey);
      return this.Ok();
    }

    [Route("api/iota/export/{reference}")]
    [HttpGet]
    public async Task<IActionResult> ResolveToSeedAsync(string reference)
    {
      var seed = await this.SeedManager.ResolveReferenceAsync(reference);
      return this.Ok(seed.Value);
    }
  }
}