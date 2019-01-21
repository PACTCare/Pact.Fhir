﻿namespace Pact.Fhir.Core.Usecase
{
  public abstract class UsecaseResponse
  {
    public ResponseCode Code { get; set; }

    public string ExceptionMessage { get; set; }
  }
}