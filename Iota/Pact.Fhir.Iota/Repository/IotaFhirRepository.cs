namespace Pact.Fhir.Iota.Repository
{
  using System;
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
  using Task = System.Threading.Tasks.Task;

  /// <summary>
  /// Inject repository for now. Core Factory needs to be adjusted or injection has to be done another way, later
  /// </summary>
  public class IotaFhirRepository : IFhirRepository
  {
    public IotaFhirRepository(
      IIotaRepository repository,
      IFhirTryteSerializer serializer,
      IResourceTracker resourceTracker,
      IChannelCredentialProvider channelCredentialProvider)
    {
      this.Serializer = serializer;
      this.ResourceTracker = resourceTracker;
      this.ChannelCredentialProvider = channelCredentialProvider;
      this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, repository);
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
    }

    // Working with low security level for the sake of speed
    public static int SecurityLevel => Tangle.Net.Cryptography.SecurityLevel.Low;

    private IChannelCredentialProvider ChannelCredentialProvider { get; }

    private MamChannelFactory ChannelFactory { get; }

    private IResourceTracker ResourceTracker { get; }

    private IFhirTryteSerializer Serializer { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task<Resource> CreateResourceAsync(Resource resource)
    {
      var channelCredentials = await this.ChannelCredentialProvider.CreateAsync();

      // New FHIR resources SHALL be assigned a logical and a version id. Take root of first message for that
      resource.PopulateMetadata(channelCredentials.RootHash.Value, channelCredentials.RootHash.Value);

      var channel = this.ChannelFactory.Create(Mode.Restricted, channelCredentials.Seed, SecurityLevel, channelCredentials.ChannelKey);
      var message = channel.CreateMessage(this.Serializer.Serialize(resource));
      await channel.PublishAsync(message, 14, 1);

      // After successfully publishing a message, we can save that to the ResourceTracker.
      // This will allow us to retrieve the channel and subscription for other usecases
      await this.ResourceTracker.AddEntryAsync(
        new ResourceEntry
          {
            ResourceRoots = new List<string> { channelCredentials.RootHash.Value },
            Channel = channel,
            Subscription = this.SubscriptionFactory.Create(channelCredentials.RootHash, Mode.Restricted, channelCredentials.ChannelKey)
          });

      return resource;
    }

    /// <inheritdoc />
    public Task DeleteResourceAsync(string id)
    {
      throw new UnsupportedOperationException("Delete");
    }

    /// <inheritdoc />
    public List<CapabilityStatement.ResourceComponent> GetCapabilities()
    {
      var interactions = new List<CapabilityStatement.ResourceInteractionComponent>
                           {
                             new CapabilityStatement.ResourceInteractionComponent { Code = CapabilityStatement.TypeRestfulInteraction.Create },
                             new CapabilityStatement.ResourceInteractionComponent { Code = CapabilityStatement.TypeRestfulInteraction.Read },
                             new CapabilityStatement.ResourceInteractionComponent { Code = CapabilityStatement.TypeRestfulInteraction.Vread },
                             new CapabilityStatement.ResourceInteractionComponent { Code = CapabilityStatement.TypeRestfulInteraction.Update },
                             new CapabilityStatement.ResourceInteractionComponent { Code = CapabilityStatement.TypeRestfulInteraction.HistoryInstance },
                             new CapabilityStatement.ResourceInteractionComponent { Code = CapabilityStatement.TypeRestfulInteraction.Patch },
                           };

      var components = new List<CapabilityStatement.ResourceComponent>();
      foreach (var resource in Enum.GetValues(typeof(ResourceType)))
      {
        components.Add(
          new CapabilityStatement.ResourceComponent
            {
              Type = (ResourceType)resource,
              Interaction = interactions,
              Versioning = CapabilityStatement.ResourceVersionPolicy.Versioned,
              ReadHistory = true,
            });
      }

      return components;
    }

    /// <inheritdoc />
    public async Task<Resource> ReadResourceAsync(string id)
    {
      // Get the tracked resource associated with the given id and get subscription from that
      var resourceEntry = await this.LoadResourceEntry(id);

      // Fetch all messages
      var messages = await FetchStreamMessagesAsync(id, resourceEntry);

      // Update the tracked subscription with the latest information
      await this.ResourceTracker.UpdateEntryAsync(resourceEntry);

      // Return the last message, since it contains the latest resource entry
      return this.Serializer.Deserialize<Resource>(messages.Last().Message);
    }

    /// <inheritdoc />
    public async Task<List<Resource>> ReadResourceHistoryAsync(string id)
    {
      // Get the tracked resource associated with the given id and get subscription from that
      var resourceEntry = await this.LoadResourceEntry(id);

      // Fetch all messages
      var messages = await FetchStreamMessagesAsync(id, resourceEntry);

      // Update the tracked subscription with the latest information
      await this.ResourceTracker.UpdateEntryAsync(resourceEntry);

      return messages.Select(m => this.Serializer.Deserialize<Resource>(m.Message)).ToList();
    }

    /// <inheritdoc />
    public async Task<Resource> ReadResourceVersionAsync(string versionId)
    {
      // Get the tracked resource associated with the given id and get subscription from that
      var resourceEntry = await this.LoadResourceEntry(versionId);

      // Get root that corresponds to the desired version id
      var resourceRoot = resourceEntry.ResourceRoots.FirstOrDefault(r => r.Contains(versionId));

      UnmaskedAuthenticatedMessage message;
      if (string.IsNullOrEmpty(resourceRoot))
      {
        // Root has not been fetched yet. Fetch stream and select desired version
        var messages = await FetchStreamMessagesAsync(versionId, resourceEntry);
        message = messages.FirstOrDefault(m => m.Root.Value.Contains(versionId));
      }
      else
      {
        // Root has been fetched before. Fetch single message
        message = await resourceEntry.Subscription.FetchSingle(new Hash(resourceRoot));
      }

      if (message == null)
      {
        throw new ResourceNotFoundException(versionId);
      }

      // Update the tracked subscription with the latest information
      await this.ResourceTracker.UpdateEntryAsync(resourceEntry);

      // Return the last message, since it contains the latest resource entry
      return this.Serializer.Deserialize<Resource>(message.Message);
    }

    /// <inheritdoc />
    public async Task<Resource> UpdateResourceAsync(Resource resource)
    {
      // get the MAM channel information for the given resource and check if it exists
      var resourceEntry = await this.LoadResourceEntry(resource.Id);
      if (resourceEntry.Channel == null)
      {
        // System does not have write access to the stream. Therefore no update can be made
        throw new AuthorizationRequiredException(resource.Id);
      }

      // populate the metadata with the new version id and add it to the resource tracker entry
      resource.PopulateMetadata(resource.Id, resourceEntry.Channel.NextRoot.Value);
      resourceEntry.ResourceRoots.Add(resourceEntry.Channel.NextRoot.Value);

      // upload data to tangle
      var message = resourceEntry.Channel.CreateMessage(this.Serializer.Serialize(resource));
      await resourceEntry.Channel.PublishAsync(message, 14, 1);

      // after everything is done, update the state of our channel
      await this.ResourceTracker.UpdateEntryAsync(resourceEntry);

      return resource;
    }

    private static async Task<List<UnmaskedAuthenticatedMessage>> FetchStreamMessagesAsync(string id, ResourceEntry resourceEntry)
    {
      var messages = await resourceEntry.Subscription.FetchAsync();
      if (messages.Count == 0)
      {
        throw new ResourceNotFoundException(id);
      }

      foreach (var message in messages)
      {
        if (!resourceEntry.ResourceRoots.Any(r => r.Contains(message.Root.Value)))
        {
          resourceEntry.ResourceRoots.Add(message.Root.Value);
        }
      }

      return messages;
    }

    private async Task<ResourceEntry> LoadResourceEntry(string id)
    {
      var resourceEntry = await this.ResourceTracker.GetEntryAsync(id);
      if (resourceEntry == null)
      {
        throw new ResourceNotFoundException(id);
      }

      return resourceEntry;
    }
  }
}