namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Repository;

  public class ResourceImporter
  {
    public ResourceImporter(ISearchRepository searchRepository, IFhirRepository fhirRepository, ISeedManager seedManager)
    {
      this.SearchRepository = searchRepository;
      this.FhirRepository = fhirRepository;
      this.SeedManager = seedManager;
    }

    private IFhirRepository FhirRepository { get; }
    private ISearchRepository SearchRepository { get; }
    private ISeedManager SeedManager { get; }

    public async Task ImportResourceAccessAsync(string root, string channelKey)
    {
      var resourceId = await this.SeedManager.ImportChannelReadAccessAsync(root, channelKey);

      var resource = await this.FhirRepository.ReadResourceAsync(resourceId);
      await this.SearchRepository.AddResourceAsync(resource);
    }
  }
}