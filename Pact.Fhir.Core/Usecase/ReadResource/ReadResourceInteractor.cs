namespace Pact.Fhir.Core.Usecase.ReadResource
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  /// <summary>
  /// see https://www.hl7.org/fhir/http.html#read
  /// </summary>
  public class ReadResourceInteractor : UsecaseInteractor<ReadResourceRequest, UsecaseResponse>
  {
    /// <inheritdoc />
    public ReadResourceInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<UsecaseResponse> ExecuteAsync(ReadResourceRequest request)
    {
      try
      {
        var resource = await this.Repository.ReadResourceAsync(request.ResourceId);
        if (resource.ResourceType.ToString() != request.ResourceType)
        {
          throw new ResourceNotFoundException(request.ResourceId);
        }

        return new UsecaseResponse { Code = ResponseCode.Success, Resource = resource };
      }
      catch (ResourceNotFoundException exception)
      {
        return new UsecaseResponse { Code = ResponseCode.ResourceNotFound, ExceptionMessage = exception.Message };
      }
      catch (Exception)
      {
        return new UsecaseResponse
        {
                   Code = ResponseCode.Failure, ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}