namespace Pact.Fhir.Core.Usecase.ReadResourceHistory
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  public class ReadResourceHistoryResponse : BaseResponse
  {
    public List<DomainResource> Resources { get; set; }
  }
}