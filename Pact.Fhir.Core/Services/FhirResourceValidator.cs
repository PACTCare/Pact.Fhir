namespace Pact.Fhir.Core.Services
{
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  using Hl7.Fhir.Model;

  public static class FhirResourceValidator
  {
    public static IEnumerable<ValidationResult> Validate(Resource resource)
    {
      return resource.Validate(new ValidationContext(resource));
    }
  }
}