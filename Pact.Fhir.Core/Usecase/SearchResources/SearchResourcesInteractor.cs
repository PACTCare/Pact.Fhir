namespace Pact.Fhir.Core.Usecase.SearchResources
{
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Web;

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

      var parameters = HttpUtility.ParseQueryString(request.Parameters);
      var filteredResources = FilterResourcesByStandardParameters(parameters, resources);
      filteredResources = FilterByNonStandardParameters(parameters, filteredResources);

      return new ResourceResponse
               {
                 Code = ResponseCode.Success,
                 Resource = new Bundle
                              {
                                Entry = new List<Bundle.EntryComponent>(filteredResources.Select(r => new Bundle.EntryComponent { Resource = r }))
                              }
               };
    }

    private static List<Resource> FilterByNonStandardParameters(NameValueCollection parameters, List<Resource> resources)
    {
      var filteredResources = new List<Resource>();
      if (parameters.AllKeys.Any(k => k == "_reference"))
      {
        var requestedReference = parameters.Get("_reference");
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

      return filteredResources;
    }

    private static List<Resource> FilterResourcesByStandardParameters(NameValueCollection parameters, List<Resource> resources)
    {
      var filteredResources = resources;

      for (var i = 0; i < parameters.Count; i++)
      {
        switch (parameters.AllKeys[i])
        {
          case "_id":
            filteredResources = filteredResources.Where(r => r.Id == parameters.Get(i)).ToList();
            break;
          case "_tag":
            var tagPayload = parameters.Get(i).Split('|');
            filteredResources = filteredResources.Where(r => r.Meta.Tag.Any(t => t.Code == tagPayload[0] && t.System == tagPayload[1])).ToList();
            break;
        }
      }

      return filteredResources;
    }
  }
}