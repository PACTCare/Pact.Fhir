namespace Pact.Fhir.Core.Usecase
{
  using System;

  public class UsecaseResponse
  {
    public ResponseCode Code { get; set; }

    public Exception Exception { get; set; }
  }
}