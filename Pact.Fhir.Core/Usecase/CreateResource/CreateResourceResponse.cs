namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using System;

  public class CreateResourceResponse : UsecaseResponse
  {
    public DateTimeOffset? LastModified { get; set; }

    public string LogicalId { get; set; }

    public string VersionId { get; set; }
  }
}