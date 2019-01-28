namespace Pact.Fhir.Core.Usecase.VersionReadResource
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  public class VersionReadResourceInteractor : UsecaseInteractor<VersionReadResourceRequest, UsecaseResponse>
  {
    /// <inheritdoc />
    public VersionReadResourceInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<UsecaseResponse> ExecuteAsync(VersionReadResourceRequest request)
    {
      try
      {
        var resource = await this.Repository.ReadResourceVersionAsync(request.VersionId);
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
                   Code = ResponseCode.Failure,
                   ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}