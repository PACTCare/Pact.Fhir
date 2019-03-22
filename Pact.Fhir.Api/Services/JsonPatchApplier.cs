namespace Pact.Fhir.Api.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Http.Formatting;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Microsoft.AspNetCore.JsonPatch;
  using Microsoft.AspNetCore.JsonPatch.Operations;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Services;

  public class JsonPatchApplier : IPatchApplier
  {
    /// <inheritdoc />
    public Resource ApplyTo(Resource resource, List<PatchOperation> operations)
    {
      var patchDocument = new JsonPatchDocument(
        operations.Select(o => new Operation(o.Operation, o.Path, string.Empty, o.Value)).ToList(),
        new JsonContractResolver(new JsonMediaTypeFormatter()));

      var serializedResource = resource.ToJson();
      patchDocument.ApplyTo(serializedResource);

      return new FhirJsonParser().Parse<Resource>(serializedResource);
    }
  }
}