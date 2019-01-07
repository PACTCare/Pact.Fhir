namespace Pact.Fhir.Iota.Repository
{
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Iota.Serializer;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.Repository;

  /// <summary>
  /// Inject repository for now. Core Factory needs to be adjusted or injection has to be done another way, later
  /// </summary>
  public class IotaFhirRepository : IFhirRepository
  {
    public IotaFhirRepository(IIotaRepository repository, IFhirTryteSerializer serializer)
    {
      this.Serializer = serializer;
      this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, repository);
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
    }

    private MamChannelFactory ChannelFactory { get; }

    private IFhirTryteSerializer Serializer { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task<DomainResource> CreateResourceAsync(DomainResource resource)
    {
      // Setup for unlinked resources (not linked to a user seed)
      // User seed handling has to be implemented later (must conform FHIR specifications)
      var seed = Seed.Random();
      var channelKey = Seed.Random();

      // New FHIR resources must be assigned a logical and a version id. Take hash of first message for that
      var rootTree = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, SecurityLevel.Low);
      resource.Id = rootTree.Root.Hash.Value;
      resource.VersionId = rootTree.Root.Hash.Value;

      // Working with low security level for the sake of speed
      // TODO: Must be changed later!
      var channel = this.ChannelFactory.Create(Mode.Restricted, seed, SecurityLevel.Low, channelKey);
      var message = channel.CreateMessage(this.Serializer.Serialize(resource));
      await channel.PublishAsync(message);

      return resource;
    }

    /// <inheritdoc />
    public Task<DomainResource> ReadResourceAsync(string id)
    {
      return null;
    }
  }
}