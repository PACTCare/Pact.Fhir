namespace Pact.Fhir.Core.Usecase.ReadResourceHistory
{
  using System.Linq;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  using Exception = System.Exception;

  public class ReadResourceHistoryInteractor : UsecaseInteractor<ReadResourceHistoryRequest, ReadResourceHistoryResponse>
  {
    /// <inheritdoc />
    public ReadResourceHistoryInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<ReadResourceHistoryResponse> ExecuteAsync(ReadResourceHistoryRequest request)
    {
      try
      {
        var resources = await this.Repository.ReadResourceHistoryAsync(request.ResourceId);
        if (resources.Count == 0 || resources.First().ResourceType.ToString() != request.ResourceType)
        {
          throw new ResourceNotFoundException(request.ResourceId);
        }

        return new ReadResourceHistoryResponse { Code = ResponseCode.Success, Resources = resources };
      }
      catch (ResourceNotFoundException exception)
      {
        return new ReadResourceHistoryResponse { Code = ResponseCode.ResourceNotFound, ExceptionMessage = exception.Message };
      }
      catch (Exception)
      {
        return new ReadResourceHistoryResponse
        {
                   Code = ResponseCode.Failure,
                   ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}
