﻿namespace Tangle.Net.Fhir.Core.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.Fhir.Core.Repository.MamStorage;
  using Tangle.Net.Fhir.Core.Repository.Responses;
  using Tangle.Net.Fhir.Core.Serializer;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.Repository;

  /// <inheritdoc />
  public class TangleFhirPatientRepository : IFhirPatientRepository
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TangleFhirPatientRepository"/> class.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="serializer">
    /// The serializer.
    /// </param>
    /// <param name="mamStorage">
    /// The mam Storage.
    /// </param>
    public TangleFhirPatientRepository(IIotaRepository repository, IFhirTryteSerializer serializer, IStatefulMam mamStorage)
    {
      this.Serializer = serializer;
      this.MamStorage = mamStorage;
      this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, repository);
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
    }

    /// <summary>
    /// Gets the channel factory.
    /// </summary>
    private MamChannelFactory ChannelFactory { get; }

    /// <summary>
    /// Gets the mam storage.
    /// </summary>
    private IStatefulMam MamStorage { get; }

    /// <summary>
    /// Gets the serializer.
    /// </summary>
    private IFhirTryteSerializer Serializer { get; }

    /// <summary>
    /// Gets the subscription factory.
    /// </summary>
    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    /// <inheritdoc />
    public async Task<ResourceReponse<T>> CreateResourceAsync<T>(T resource, Seed seed, TryteString channelKey)
      where T : DomainResource
    {
      var channel = this.ChannelFactory.Create(Mode.Restricted, seed, SecurityLevel.Medium, channelKey);

      // get the first root and assign that as ID
      var rootTree = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, SecurityLevel.Medium);
      resource.Id = rootTree.Root.Hash.Value;

      var message = channel.CreateMessage(this.Serializer.Serialize(resource));
      await channel.PublishAsync(message);

      this.MamStorage.AddChannel(channel);

      return new ResourceReponse<T> { Message = message, Resource = resource, Channel = channel };
    }

    /// <inheritdoc />
    public async Task<List<T>> GetHistory<T>(Hash root, TryteString channelKey)
      where T : DomainResource
    {
      var subscription = this.MamStorage.GetSubscription(root);

      if (subscription == null)
      {
        subscription = this.SubscriptionFactory.Create(root, Mode.Restricted, channelKey);
        this.MamStorage.AddChannelSubscription(subscription);
      }

      var messages = await subscription.FetchAsync();

      return messages.Select(m => this.Serializer.Deserialize<T>(m.Message)).ToList();
    }

    /// <inheritdoc />
    public async Task<T> GetResourceAsync<T>(Hash root, TryteString channelKey)
      where T : DomainResource
    {
      var subscription = this.MamStorage.GetSubscription(root);

      if (subscription == null)
      {
        subscription = this.SubscriptionFactory.Create(root, Mode.Restricted, channelKey);
        this.MamStorage.AddChannelSubscription(subscription);
      }
      
      var messages = await subscription.FetchAsync();

      return this.Serializer.Deserialize<T>(messages.Last().Message);
    }

    /// <inheritdoc />
    public async Task<ResourceReponse<T>> UpdateResourceAsync<T>(T resource, Seed seed)
      where T : DomainResource
    {
      var channel = this.MamStorage.GetChannel(seed);

      if (channel == null)
      {
        throw new ArgumentException("Channel not registered for given seed. Please add the channel with its current state.");
      }

      var message = channel.CreateMessage(this.Serializer.Serialize(resource));
      await channel.PublishAsync(message);

      resource.VersionId = message.Root.Value;

      return new ResourceReponse<T> { Message = message, Channel = channel, Resource = resource };
    }

    /// <inheritdoc />
    public void AddChannel(MamChannel channel)
    {
      this.MamStorage.AddChannel(channel);
    }
  }
}