namespace Pact.Fhir.Iota.Services
{
  using System;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Iota.Events;

  using Task = System.Threading.Tasks.Task;

  public class ResourceImporter
  {
    public static event EventHandler<ResourceAddedEventArgs> ResourceAdded; 

    public ResourceImporter(ISearchRepository searchRepository, IFhirRepository fhirRepository, ISeedManager seedManager)
    {
      this.SearchRepository = searchRepository;
      this.FhirRepository = fhirRepository;
      this.SeedManager = seedManager;

      DeterministicSeedManager.SubscriptionAdded += this.SubscriptionAddedInManager;
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

    private async void SubscriptionAddedInManager(object sender, SubscriptionAddedEventArgs e)
    {
      var resource = await this.FhirRepository.ReadResourceAsync(e.ResourceId);
      if (resource is Patient)
      {
        await this.SeedManager.AddReferenceAsync($"did:iota:{resource.Id}", e.Seed);
      }

      await this.SearchRepository.AddResourceAsync(resource);
      ResourceAdded?.Invoke(this, new ResourceAddedEventArgs(resource));
    }
  }
}