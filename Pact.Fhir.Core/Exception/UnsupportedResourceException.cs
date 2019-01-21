namespace Pact.Fhir.Core.Exception
{
  using System;

  public class UnsupportedResourceException : Exception
  {
    public UnsupportedResourceException(string resourceType)
      : base($"Resource of type {resourceType} is not supported.")
    {
    }
  }
}