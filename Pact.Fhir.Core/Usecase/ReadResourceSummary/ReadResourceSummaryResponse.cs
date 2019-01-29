namespace Pact.Fhir.Core.Usecase.ReadResourceSummary
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  public class ReadResourceSummaryResponse : BaseResponse
  {
    public List<DomainResource> Resources { get; set; }
  }
}