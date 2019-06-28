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
    public IotaController(ResourceImporter resourceImporter, IReferenceResolver referenceResolver)
    {
      this.ResourceImporter = resourceImporter;
      this.ReferenceResolver = referenceResolver;
    }

    private IReferenceResolver ReferenceResolver { get; }

    private ResourceImporter ResourceImporter { get; }

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
      var seed = this.ReferenceResolver.Resolve(reference);
      return this.Ok(seed.Value);
    }
  }
}