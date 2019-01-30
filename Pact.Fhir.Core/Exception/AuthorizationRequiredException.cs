namespace Pact.Fhir.Core.Exception
{
  using System;

  public class AuthorizationRequiredException : Exception
  {
    public AuthorizationRequiredException(string resourceId)
      : base($"Resource with ID {resourceId} is read only")
    {
    }
  }
}