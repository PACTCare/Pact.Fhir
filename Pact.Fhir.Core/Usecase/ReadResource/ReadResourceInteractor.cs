namespace Pact.Fhir.Core.Usecase.ReadResource
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;

  /// <summary>
  /// see https://www.hl7.org/fhir/http.html#read
  /// </summary>
  public class ReadResourceInteractor : UsecaseInteractor<ReadResourceRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public ReadResourceInteractor(IFhirRepository repository, ISearchRepository searchRepository)
      : base(repository)
    {
      this.SearchRepository = searchRepository;
    }

    private ISearchRepository SearchRepository { get; }

    /// <inheritdoc />
    public override async Task<ResourceResponse> ExecuteAsync(ReadResourceRequest request)
    {
      try
      {
        var resource = await this.Repository.ReadResourceAsync(request.ResourceId);
        if (resource.ResourceType.ToString() != request.ResourceType)
        {
          throw new ResourceNotFoundException(request.ResourceId);
        }

        await this.SearchRepository.UpdateResourceAsync(resource);

        return new ResourceResponse { Code = ResponseCode.Success, Resource = resource };
      }
      catch (ResourceNotFoundException exception)
      {
        return new ResourceResponse { Code = ResponseCode.ResourceNotFound, Exception = exception};
      }
      catch (Exception exception)
      {
        return new ResourceResponse { Code = ResponseCode.Failure, Exception = exception };
      }
    }
  }
}