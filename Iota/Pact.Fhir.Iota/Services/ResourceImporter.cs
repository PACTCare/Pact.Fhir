using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pact.Fhir.Core.Repository;
using Pact.Fhir.Iota.Entity;
using Tangle.Net.Entity;
using Tangle.Net.Mam.Entity;
using Tangle.Net.Mam.Services;
using Tangle.Net.Repository;

namespace Pact.Fhir.Iota.Services
{
  public class ResourceImporter
  {
    private IResourceTracker ResourceTracker { get; }

    private ISearchRepository SearchRepository { get; }

    private IFhirRepository FhirRepository { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    public ResourceImporter(IResourceTracker resourceTracker, IIotaRepository repository, ISearchRepository searchRepository, IFhirRepository fhirRepository)
    {
      this.ResourceTracker = resourceTracker;
      this.SearchRepository = searchRepository;
      this.FhirRepository = fhirRepository;
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
    }

    public async Task ImportResourceAccessAsync(string root, string channelKey)
    {
      var subscription = this.SubscriptionFactory.Create(new Hash(root), Mode.Restricted, channelKey, true);

      await this.ResourceTracker.AddEntryAsync(new ResourceEntry
        { ResourceRoots = new List<string> { root }, Subscription = subscription });

      var resource = await this.FhirRepository.ReadResourceAsync(root.Substring(0, 64));
      await this.SearchRepository.AddResourceAsync(resource);
    }
  }
}
