namespace Pact.Fhir.Iota.Repository
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

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
  public class IotaFhirRepository : FhirRepository
  {
    public IotaFhirRepository(IIotaRepository repository, IFhirTryteSerializer serializer, IResourceTracker resourceTracker)
    {
      this.Serializer = serializer;
      this.ResourceTracker = resourceTracker;
      this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, repository);
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
    }

    // Working with low security level for the sake of speed
    private static int SecurityLevel => Tangle.Net.Cryptography.SecurityLevel.Medium;

    private MamChannelFactory ChannelFactory { get; }

    private IResourceTracker ResourceTracker { get; }

    private IFhirTryteSerializer Serializer { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public override async Task<DomainResource> CreateResourceAsync(DomainResource resource)
    {
      // Setup for unlinked resources (not linked to a user seed)
      // User seed handling has to be implemented later (must conform FHIR specifications)
      var seed = Seed.Random();
      var channelKey = Seed.Random();

      // New FHIR resources SHALL be assigned a logical and a version id. Take hash of first message for that
      var rootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, SecurityLevel).Root.Hash;
      this.PopulateMetadata(resource, rootHash.Value, rootHash.Value);

      var channel = this.ChannelFactory.Create(Mode.Restricted, seed, SecurityLevel, channelKey);
      var message = channel.CreateMessage(this.Serializer.Serialize(resource));
      await channel.PublishAsync(message);

      // After successfully publishing a message, we can save that to the ResourceTracker.
      // This will allow us to retrieve the channel and subscription for other usecases
      this.ResourceTracker.AddEntry(
        new ResourceEntry
          {
            StreamHashes = new List<Hash> { rootHash },
            Channel = channel.ToJson(),
            Subscription = this.SubscriptionFactory.Create(rootHash, Mode.Restricted, channelKey).ToJson()
          });

      return resource;
    }

    /// <inheritdoc />
    public override async Task<DomainResource> ReadResourceAsync(string id)
    {
      // Get the tracked resource associated with the given id and filter the MAM root from that
      var resourceEntry = this.ResourceTracker.GetEntry(id);
      if (resourceEntry == null)
      {
        throw new ResourceNotFoundException(id);
      }

      // Fetch all messages
      var messages = await resourceEntry.GetSubscription(this.SubscriptionFactory).FetchAsync();
      if (messages.Count == 0)
      {
        throw new ResourceNotFoundException(id);
      }

      // TODO: update resourceEntry with newest subscription information

      // Return the last message, since it contains the latest resource entry
      return this.Serializer.Deserialize<DomainResource>(messages.Last().Message);
    }
  }
}