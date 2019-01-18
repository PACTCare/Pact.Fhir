namespace Pact.Fhir.Core.Exception
{
  using System;

  public class UnsupportedResourceException : Exception
  {
    public UnsupportedResourceException(string message)
      : base(message)
    {
    }
  }
}