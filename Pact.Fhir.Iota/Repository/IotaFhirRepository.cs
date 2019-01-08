namespace Pact.Fhir.Iota.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.Repository;

  using ResourceEntry = Entity.ResourceEntry;

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
      var rootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, SecurityLevel.Low).Root.Hash;
      this.PopulateMetadata(resource, rootHash.Value, rootHash.Value);

      // Working with low security level for the sake of speed
      // TODO: Must be changed later!
      var channel = this.ChannelFactory.Create(Mode.Restricted, seed, SecurityLevel.Low, channelKey);
      var message = channel.CreateMessage(this.Serializer.Serialize(resource));
      await channel.PublishAsync(message);

      // After successfully publishing a message, we can save that to the ResourceTracker.
      // This will allow us to retrieve the channelKey for other usecases
      this.ResourceTracker.AddEntry(new ResourceEntry { ChannelKey = channelKey, MerkleRoots = new List<TryteString> { rootHash } });

      return resource;
    }

    /// <inheritdoc />
    public override async Task<DomainResource> ReadResourceAsync(string id)
    {
      return null;
    }
  }
}