namespace Pact.Fhir.Iota.Repository
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.Repository;

  using ResourceEntry = Pact.Fhir.Iota.Entity.ResourceEntry;

  /// <summary>
  /// Inject repository for now. Core Factory needs to be adjusted or injection has to be done another way, later
  /// </summary>
  public class IotaFhirRepository : IFhirRepository
  {
    public IotaFhirRepository(IIotaRepository repository, IFhirTryteSerializer serializer, IResourceTracker resourceTracker)
    {
      this.Serializer = serializer;
      this.ResourceTracker = resourceTracker;
      this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, repository);
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
    }

    // Working with low security level for the sake of speed
    private static int SecurityLevel => Tangle.Net.Cryptography.SecurityLevel.Low;

    private MamChannelFactory ChannelFactory { get; }

    private IResourceTracker ResourceTracker { get; }

    private IFhirTryteSerializer Serializer { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task<DomainResource> CreateResourceAsync(DomainResource resource)
    {
      // Setup for unlinked resources (not linked to a user seed)
      // User seed handling has to be implemented later (must conform FHIR specifications)
      var seed = Seed.Random();
      var channelKey = Seed.Random();

      // New FHIR resources SHALL be assigned a logical and a version id. Take hash of first message for that
      var rootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, SecurityLevel).Root.Hash;
      resource.PopulateMetadata(rootHash.Value, rootHash.Value);

      var channel = this.ChannelFactory.Create(Mode.Restricted, seed, SecurityLevel, channelKey);
      var message = channel.CreateMessage(this.Serializer.Serialize(resource));
      await channel.PublishAsync(message, 14, 1);

      // After successfully publishing a message, we can save that to the ResourceTracker.
      // This will allow us to retrieve the channel and subscription for other usecases
      await this.ResourceTracker.AddEntryAsync(
        new ResourceEntry
          {
            ResourceIds = new List<string> { resource.Id },
            Channel = channel,
            Subscription = this.SubscriptionFactory.Create(rootHash, Mode.Restricted, channelKey)
          });

      return resource;
    }

    /// <inheritdoc />
    public async Task<DomainResource> ReadResourceAsync(string id)
    {
      // Get the tracked resource associated with the given id and get subscription from that
      var resourceEntry = await this.ResourceTracker.GetEntryAsync(id);
      if (resourceEntry == null)
      {
        throw new ResourceNotFoundException(id);
      }

      // Fetch all messages
      var messages = await resourceEntry.Subscription.FetchAsync();
      if (messages.Count == 0)
      {
        throw new ResourceNotFoundException(id);
      }

      // Update the tracked subscription with the latest information
      await this.ResourceTracker.UpdateEntryAsync(resourceEntry);

      // Return the last message, since it contains the latest resource entry
      return this.Serializer.Deserialize<DomainResource>(messages.Last().Message);
    }

    /// <inheritdoc />
    public Task<DomainResource> ReadResourceVersionAsync(string versionId)
    {
      return null;
    }

    /// <inheritdoc />
    public async Task<DomainResource> UpdateResourceAsync(DomainResource resource)
    {
      // get the MAM channel information for the given resource and check if it exists
      var resourceEntry = await this.ResourceTracker.GetEntryAsync(resource.Id);
      if (resourceEntry == null)
      {
        throw new ResourceNotFoundException(resource.Id);
      }

      if (resourceEntry.Channel == null)
      {
        // System does not have write access to the stream. Therefore no update can be made
        throw new AuthorizationRequiredException(resource.Id);
      }

      // populate the metadata with the new version id and add it to the resource tracker entry
      resource.PopulateMetadata(resource.Id, resourceEntry.Channel.NextRoot.Value);
      resourceEntry.ResourceIds.Add(resource.Meta.VersionId);

      // upload data to tangle
      var message = resourceEntry.Channel.CreateMessage(this.Serializer.Serialize(resource));
      await resourceEntry.Channel.PublishAsync(message, 14, 1);

      // after everything is done, update the state of our channel
      await this.ResourceTracker.UpdateEntryAsync(resourceEntry);

      return resource;
    }
  }
}