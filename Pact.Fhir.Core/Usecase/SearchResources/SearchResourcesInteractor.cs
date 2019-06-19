namespace Pact.Fhir.Core.Usecase.SearchResources
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;

  public class SearchResourcesInteractor : UsecaseInteractor<SearchResourcesRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public SearchResourcesInteractor(IFhirRepository repository, ISearchRepository searchRepository)
      : base(repository)
    {
      this.SearchRepository = searchRepository;
    }

    private ISearchRepository SearchRepository { get; }

    /// <inheritdoc />
    public override async Task<ResourceResponse> ExecuteAsync(SearchResourcesRequest request)
    {
      if (string.IsNullOrEmpty(request.Parameters))
      {
        var resources = await this.SearchRepository.FindResourcesByTypeAsync(request.ResourceType);

        return new ResourceResponse
                 {
                   Code = ResponseCode.Success,
                   Resource = new Bundle
                                {
                                  Entry = new List<Bundle.EntryComponent>(resources.Select(r => new Bundle.EntryComponent { Resource = r }))
                                }
                 };
      }

      return new ResourceResponse();
    }
  }
}