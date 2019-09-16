namespace Pact.Fhir.Iota.Events
{
  using System;

  using Hl7.Fhir.Model;

  public class ResourceAddedEventArgs : EventArgs
  {
    public ResourceAddedEventArgs(Resource resource)
    {
      this.Resource = resource;
    }

    public Resource Resource { get; }
  }
}