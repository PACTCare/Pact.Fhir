namespace Pact.Fhir.Core.Exception
{
  using System;

  public class ResourceNotFoundException : Exception
  {
    public ResourceNotFoundException(string id)
      : base($"Requested resource with id {id} not found")
    {
      this.Id = id;
    }

    public string Id { get; }
  }
}