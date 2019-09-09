namespace Pact.Fhir.Core.Usecase.DeleteResource
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  public class DeleteResourceInteractor : UsecaseInteractor<DeleteResourceRequest, UsecaseResponse>
  {
    /// <inheritdoc />
    public DeleteResourceInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<UsecaseResponse> ExecuteAsync(DeleteResourceRequest request)
    {
      try
      {
        await this.Repository.DeleteResourceAsync(request.ResourceId);
        return new UsecaseResponse { Code = ResponseCode.Success };
      }
      catch (ResourceNotFoundException exception)
      {
        return new UsecaseResponse { Code = ResponseCode.ResourceNotFound, Exception = exception };
      }
      catch (Exception exception)
      {
        return new UsecaseResponse { Code = ResponseCode.Failure, Exception = exception };
      }
    }
  }
}