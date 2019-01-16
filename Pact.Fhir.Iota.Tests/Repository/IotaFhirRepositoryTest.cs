namespace Pact.Fhir.Iota.Tests.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;

  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Tests.Services;
  using Pact.Fhir.Iota.Tests.Utils;

  using Tangle.Net.Cryptography;
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
    public async Task TestResourceCreationOnTangleShouldAssignHashesAsIds()
    {
      var resourceTracker = new InMemoryResourceTracker();
      var repository = new IotaFhirRepository(IotaResourceProvider.Repository, new FhirJsonTryteSerializer(), resourceTracker);
      var resource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);

      Assert.AreEqual(1, Regex.Matches(resource.Id, Id.PATTERN).Count);
      Assert.AreEqual(resource.Id, resource.VersionId);
      Assert.AreEqual(resource.VersionId, resource.Meta.VersionId);

      Assert.AreEqual(DateTime.UtcNow.Day, resource.Meta.LastUpdated?.DateTime.Day);
      Assert.AreEqual(DateTime.UtcNow.Month, resource.Meta.LastUpdated?.DateTime.Month);
      Assert.AreEqual(DateTime.UtcNow.Year, resource.Meta.LastUpdated?.DateTime.Year);

      Assert.IsTrue(InputValidator.IsTrytes(resource.Id));
      Assert.AreEqual(1, resourceTracker.Entries.Count);
    }

    [TestMethod]
    public async Task TestResourceCanBeReadFromTangle()
    {
      var repository = new IotaFhirRepository(IotaResourceProvider.Repository, new FhirJsonTryteSerializer(), new InMemoryResourceTracker());
      var createdResource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);
      var readResource = await repository.ReadResourceAsync(createdResource.Id);

      Assert.IsTrue(createdResource.IsExactly(readResource));
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public async Task TestResourceIsNotRegisteredInTrackerShouldThrowException()
    {
      var repository = new IotaFhirRepository(IotaResourceProvider.Repository, new FhirJsonTryteSerializer(), new InMemoryResourceTracker());
      await repository.ReadResourceAsync("SOMEID");
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public async Task TestNoMessagesAreOnMamStreamShouldThrowException()
    {
      var iotaRepository = IotaResourceProvider.Repository;
      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var resourceTracker = new InMemoryResourceTracker();
      resourceTracker.AddEntry(
        new ResourceEntry
          {
            Channel = channelFactory.Create(Mode.Restricted, Seed.Random(), SecurityLevel.Medium, Seed.Random()),
            Subscription = subscriptionFactory.Create(new Hash(Seed.Random().Value), Mode.Restricted, Seed.Random()),
            StreamHashes = new List<Hash> { new Hash("SOMEID") }
          });

      var repository = new IotaFhirRepository(iotaRepository, new FhirJsonTryteSerializer(), resourceTracker);
      await repository.ReadResourceAsync("SOMEID");
    }
  }
}