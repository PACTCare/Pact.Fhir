namespace Pact.Fhir.Core.Usecase
{
  public class BaseResponse
  {
    public ResponseCode Code { get; set; }

    public string ExceptionMessage { get; set; }
  }
}