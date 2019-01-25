namespace Pact.Fhir.Core.Usecase
{
  public enum ResponseCode
  {
    Success = 1,

    Failure = -1,

    ResourceNotFound = -2,

    UnsupportedResource = -3,

    UnprocessableEntity = -4,

    IdMismatch = -5
  }
}