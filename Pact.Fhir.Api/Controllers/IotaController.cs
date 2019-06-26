using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Pact.Fhir.Api.Models;
using Pact.Fhir.Iota.Services;

namespace Pact.Fhir.Api.Controllers
{
  [EnableCors("Development")]
  [ApiController]
  public class IotaController : Controller
  {
    private ResourceImporter ResourceImporter { get; }
    private IReferenceResolver ReferenceResolver { get; }

    public IotaController(ResourceImporter resourceImporter, IReferenceResolver referenceResolver)
    {
      ResourceImporter = resourceImporter;
      ReferenceResolver = referenceResolver;
    }

    [Route("api/iota/import")]
    [HttpPost]
    public async Task<IActionResult> ImportResourceAccess([FromBody] MamKeyPair mamKeyPair)
    {
      await this.ResourceImporter.ImportResourceAccessAsync(mamKeyPair.Root, mamKeyPair.ChannelKey);
      return this.Ok();
    }

    [Route("api/iota/export/{reference}")]
    [HttpGet]
    public async Task<IActionResult> ResolveToSeed(string reference)
    {
      var seed = this.ReferenceResolver.Resolve(reference);
      return this.Ok(seed.Value);
    }
  }
}