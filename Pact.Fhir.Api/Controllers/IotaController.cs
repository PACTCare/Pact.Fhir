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

    public IotaController(ResourceImporter resourceImporter)
    {
      ResourceImporter = resourceImporter;
    }

    [Route("api/iota/import")]
    [HttpPost]
    public async Task<IActionResult> ImportResourceAccess([FromBody] MamKeyPair mamKeyPair)
    {
      await this.ResourceImporter.ImportResourceAccessAsync(mamKeyPair.Root, mamKeyPair.ChannelKey);
      return this.Ok();
    }
  }
}