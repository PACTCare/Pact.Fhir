namespace Pact.Fhir.Core.Usecase.ReadResourceVersion
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  public class ReadResourceVersionInteractor : UsecaseInteractor<ReadResourceVersionRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public ReadResourceVersionInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    /// <inheritdoc />
    public override async Task<ResourceResponse> ExecuteAsync(ReadResourceVersionRequest request)
    {
      try
      {
        var resource = await this.Repository.ReadResourceVersionAsync(request.VersionId);
        if (resource.ResourceType.ToString() != request.ResourceType)
        {
          throw new ResourceNotFoundException(request.ResourceId);
        }

        return new ResourceResponse { Code = ResponseCode.Success, Resource = resource };
      }
      catch (ResourceNotFoundException exception)
      {
        return new ResourceResponse { Code = ResponseCode.ResourceNotFound, Exception = exception };
      }
      catch (Exception exception)
      {
        return new ResourceResponse { Code = ResponseCode.Failure, Exception = exception };
      }
    }
  }
}