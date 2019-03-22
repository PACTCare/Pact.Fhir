namespace Pact.Fhir.Core.Usecase.ValidateResource
{
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  public class ValidateResourceResponse : UsecaseResponse
  {
    public List<ValidationResult> ValidationResult { get; set; }
  }
}