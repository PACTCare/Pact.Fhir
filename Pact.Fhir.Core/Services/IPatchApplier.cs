namespace Pact.Fhir.Core.Services
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Entity;

  public interface IPatchApplier
  {
    Resource ApplyTo(Resource resource, List<PatchOperation> operations);
  }
}