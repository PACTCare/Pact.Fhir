namespace Pact.Fhir.Iota.SqlLite.Tests.Services
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Iota.Entity;
  using Pact.Fhir.Iota.SqlLite.Encryption;
  using Pact.Fhir.Iota.SqlLite.Services;
  using Pact.Fhir.Iota.Tests.Utils;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;

  [TestClass]
  public class SqlLiteResourceTrackerTest
  {
    [TestMethod]
    public async Task TestEntryCanBeWrittenAndRead()
    {
      var iotaRepository = IotaResourceProvider.Repository;
      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var tracker = new SqlLiteResourceTracker(channelFactory, subscriptionFactory, new RijndaelEncryption(Seed.Random().Value, Seed.Random().Value));

      // fhir id will only be 64 chars long
      var resourceId = Seed.Random().Value.Substring(0, 64);
      await tracker.AddEntryAsync(
        new ResourceEntry
          {
            Channel = channelFactory.Create(Mode.Restricted, Seed.Random(), SecurityLevel.Medium, Seed.Random().Value),
            Subscription = subscriptionFactory.Create(new Hash(Seed.Random().Value), Mode.Restricted, Seed.Random().Value),
            ResourceRoots = new List<string> { resourceId }
          });

      var resource = await tracker.GetEntryAsync(resourceId);
      Assert.IsTrue(resource.ResourceRoots.FirstOrDefault(h => h == resourceId) != null);
    }

    [TestMethod]
    public async Task TestResourceUpdateAddsAndUpdatesInformation()
    {
      var iotaRepository = IotaResourceProvider.Repository;
      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var tracker = new SqlLiteResourceTracker(channelFactory, subscriptionFactory, new RijndaelEncryption(Seed.Random().Value, Seed.Random().Value));

      // fhir id will only be 64 chars long
      var resourceId = Seed.Random().Value.Substring(0, 64);
      await tracker.AddEntryAsync(
        new ResourceEntry
          {
            Channel = channelFactory.Create(Mode.Restricted, Seed.Random(), SecurityLevel.Medium, Seed.Random().Value),
            Subscription = subscriptionFactory.Create(new Hash(Seed.Random().Value), Mode.Restricted, Seed.Random().Value),
            ResourceRoots = new List<string> { resourceId }
          });

      var resource = await tracker.GetEntryAsync(resourceId);
      resource.ResourceRoots.Add(Seed.Random().Value.Substring(0, 64));

      await tracker.UpdateEntryAsync(resource);

      Assert.AreEqual(2, (await tracker.GetEntryAsync(resourceId)).ResourceRoots.Count);
    }
  }
}