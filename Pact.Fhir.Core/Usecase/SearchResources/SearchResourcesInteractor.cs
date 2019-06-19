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
      var resources = await this.SearchRepository.FindResourcesByTypeAsync(request.ResourceType);

      if (string.IsNullOrEmpty(request.Parameters))
      {
        return new ResourceResponse
                 {
                   Code = ResponseCode.Success,
                   Resource = new Bundle
                                {
                                  Entry = new List<Bundle.EntryComponent>(resources.Select(r => new Bundle.EntryComponent { Resource = r }))
                                }
                 };
      }

      var filteredResources = new List<Resource>();

      if (request.Parameters.Contains("reference"))
      {
        var requestedReference = request.Parameters.Split('=')[1];
        foreach (var resource in resources)
        {
          if (resource.GetType().GetProperty("Subject") != null || resource.GetType().GetProperty("Patient") != null)
          {
            var reference = resource.GetType().GetProperty("Subject") != null
                              ? resource.GetType().GetProperty("Subject")?.GetValue(resource)
                              : resource.GetType().GetProperty("Patient")?.GetValue(resource);

            if (reference != null && reference is ResourceReference resourceReference && resourceReference.Reference.Contains(requestedReference))
            {
              filteredResources.Add(resource);
            }
          }
        }
      }

      return new ResourceResponse
               {
                 Code = ResponseCode.Success,
                 Resource = new Bundle
                              {
                                Entry = new List<Bundle.EntryComponent>(filteredResources.Select(r => new Bundle.EntryComponent { Resource = r }))
                              }
               };
    }
  }
}