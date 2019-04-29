namespace Pact.Fhir.Iota.Tests.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;

  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.Tests.Services;
  using Pact.Fhir.Iota.Tests.Utils;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Curl;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.Utils;

  using ResourceEntry = Pact.Fhir.Iota.Entity.ResourceEntry;
  using Task = System.Threading.Tasks.Task;

  [TestClass]
  public class IotaFhirRepositoryTest
  {
    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public async Task TestNoMessagesAreOnMamStreamShouldThrowException()
    {
      var iotaRepository = IotaResourceProvider.Repository;
      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var resourceTracker = new InMemoryResourceTracker();
      await resourceTracker.AddEntryAsync(
        new ResourceEntry
          {
            Channel = channelFactory.Create(Mode.Restricted, Seed.Random(), SecurityLevel.Medium, Seed.Random().Value),
            Subscription = subscriptionFactory.Create(new Hash(Seed.Random().Value), Mode.Restricted, Seed.Random().Value),
            ResourceRoots = new List<string> { "SOMEID" }
          });

      var repository = new IotaFhirRepository(
        iotaRepository,
        new FhirJsonTryteSerializer(),
        resourceTracker,
        new RandomChannelCredentialProvider(),
        new InMemoryReferenceResolver());
      await repository.ReadResourceAsync("SOMEID");
    }

    [TestMethod]
    public async Task TestResourceCanBeReadFromTangle()
    {
      var resourceTracker = new InMemoryResourceTracker();
      var iotaRepository = IotaResourceProvider.Repository;
      var repository = new IotaFhirRepository(
        iotaRepository,
        new FhirJsonTryteSerializer(),
        resourceTracker,
        new InMemoryDeterministicCredentialProvider(
          resourceTracker,
          new IssSigningHelper(new Curl(), new Curl(), new Curl()),
          new AddressGenerator(),
          iotaRepository),
        new InMemoryReferenceResolver());

      var createdResource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);
      var readResource = await repository.ReadResourceAsync(createdResource.Id);

      Assert.IsTrue(createdResource.IsExactly(readResource));
    }

    [TestMethod]
    public async Task TestResourceCreationOnTangleShouldAssignHashesAsIds()
    {
      var resourceTracker = new InMemoryResourceTracker();
      var referenceResolver = new InMemoryReferenceResolver();
      var repository = new IotaFhirRepository(
        IotaResourceProvider.Repository,
        new FhirJsonTryteSerializer(),
        resourceTracker,
        new RandomChannelCredentialProvider(),
        referenceResolver);
      var resource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);

      Assert.AreEqual(1, Regex.Matches(resource.Id, Id.PATTERN).Count);
      Assert.AreEqual(resource.Id, resource.VersionId);
      Assert.AreEqual(resource.VersionId, resource.Meta.VersionId);

      Assert.AreEqual(DateTime.UtcNow.Day, resource.Meta.LastUpdated?.DateTime.Day);
      Assert.AreEqual(DateTime.UtcNow.Month, resource.Meta.LastUpdated?.DateTime.Month);
      Assert.AreEqual(DateTime.UtcNow.Year, resource.Meta.LastUpdated?.DateTime.Year);

      Assert.IsTrue(InputValidator.IsTrytes(resource.Id));
      Assert.AreEqual(1, resourceTracker.Entries.Count);
      Assert.IsTrue(referenceResolver.References.Any(e => e.Key == $"urn:iota:{resource.Id}"));
    }

    [TestMethod]
    public async Task TestResourceHasValidReferenceShouldUseExistingSeed()
    {
      var referenceResolver = new InMemoryReferenceResolver();
      var repository = new IotaFhirRepository(
        IotaResourceProvider.Repository,
        new FhirJsonTryteSerializer(),
        new InMemoryResourceTracker(),
        new RandomChannelCredentialProvider(),
        referenceResolver);

      var reference = $"urn:iota:JHAGDJAHDJAHGDAJHGD";
      referenceResolver.AddReference(reference, Seed.Random());

      var observation = new Observation { Subject = new ResourceReference(reference) };
      var resource = await repository.CreateResourceAsync(observation);

      Assert.AreEqual(1, referenceResolver.References.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public async Task TestResourceIsNotRegisteredInTrackerOnReadShouldThrowException()
    {
      var repository = new IotaFhirRepository(
        IotaResourceProvider.Repository,
        new FhirJsonTryteSerializer(),
        new InMemoryResourceTracker(),
        new RandomChannelCredentialProvider(),
        new InMemoryReferenceResolver());
      await repository.ReadResourceAsync("SOMEID");
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public async Task TestResourceIsNotRegisteredInTrackerOnUpdateShouldThrowException()
    {
      var repository = new IotaFhirRepository(
        IotaResourceProvider.Repository,
        new FhirJsonTryteSerializer(),
        new InMemoryResourceTracker(),
        new RandomChannelCredentialProvider(),
        new InMemoryReferenceResolver());
      await repository.UpdateResourceAsync(FhirResourceProvider.Patient);
    }

    [TestMethod]
    [ExpectedException(typeof(AuthorizationRequiredException))]
    public async Task TestResourceIsReadOnlyShouldThrowException()
    {
      var resourceTracker = new InMemoryResourceTracker();
      await resourceTracker.AddEntryAsync(new ResourceEntry { ResourceRoots = new List<string> { "SOMEID" } });

      var repository = new IotaFhirRepository(
        IotaResourceProvider.Repository,
        new FhirJsonTryteSerializer(),
        resourceTracker,
        new RandomChannelCredentialProvider(),
        new InMemoryReferenceResolver());

      var resource = FhirResourceProvider.Patient;
      resource.Id = "SOMEID";

      await repository.UpdateResourceAsync(resource);
    }

    [TestMethod]
    public async Task TestResourceIsUpdatedShouldReturnAskedVersionOnVRead()
    {
      var resourceTracker = new InMemoryResourceTracker();
      var repository = new IotaFhirRepository(
        IotaResourceProvider.Repository,
        new FhirJsonTryteSerializer(),
        resourceTracker,
        new RandomChannelCredentialProvider(),
        new InMemoryReferenceResolver());

      var createdResource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);
      var initialVersion = createdResource.VersionId;

      var updatedResource = await repository.UpdateResourceAsync(createdResource);
      var updatedVersionId = updatedResource.VersionId;

      await repository.UpdateResourceAsync(updatedResource);
      var readResource = await repository.ReadResourceVersionAsync(updatedVersionId);

      Assert.AreNotEqual(initialVersion, readResource.Meta.VersionId);
      Assert.AreEqual(updatedVersionId, readResource.Meta.VersionId);
      Assert.AreEqual(3, resourceTracker.Entries.First().ResourceRoots.Count);
    }

    [TestMethod]
    public async Task TestResourceIsUpdatedShouldReturnNewVersionOnRead()
    {
      var resourceTracker = new InMemoryResourceTracker();
      var iotaRepository = IotaResourceProvider.Repository;
      var repository = new IotaFhirRepository(
        iotaRepository,
        new FhirJsonTryteSerializer(),
        resourceTracker,
        new InMemoryDeterministicCredentialProvider(
          resourceTracker,
          new IssSigningHelper(new Curl(), new Curl(), new Curl()),
          new AddressGenerator(),
          iotaRepository),
        new InMemoryReferenceResolver());

      var createdResource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);
      var initialVersion = createdResource.Meta.VersionId;

      var updatedResource = await repository.UpdateResourceAsync(createdResource);
      var readResource = await repository.ReadResourceAsync(updatedResource.Id);

      Assert.AreNotEqual(initialVersion, readResource.Meta.VersionId);
      Assert.AreEqual(2, resourceTracker.Entries.First().ResourceRoots.Count);
    }

    [TestMethod]
    public async Task TestUpdatedResourceShouldReturnAllEntriesInHistory()
    {
      var resourceTracker = new InMemoryResourceTracker();
      var iotaRepository = IotaResourceProvider.Repository;
      var repository = new IotaFhirRepository(
        iotaRepository,
        new FhirJsonTryteSerializer(),
        resourceTracker,
        new InMemoryDeterministicCredentialProvider(
          resourceTracker,
          new IssSigningHelper(new Curl(), new Curl(), new Curl()),
          new AddressGenerator(),
          iotaRepository),
        new InMemoryReferenceResolver());

      var createdResource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);
      var updatedResource = await repository.UpdateResourceAsync(createdResource);

      var resources = await repository.ReadResourceHistoryAsync(updatedResource.Id);
      Assert.AreEqual(2, resources.Count);
    }
  }
}