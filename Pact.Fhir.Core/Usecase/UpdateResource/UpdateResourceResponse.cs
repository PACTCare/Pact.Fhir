namespace Pact.Fhir.Core.Usecase.UpdateResource
{
  using System;

  public class UpdateResourceResponse : UsecaseResponse
  {
    public DateTime LastModified { get; set; }

    public string VersionId { get; set; }
  }
}