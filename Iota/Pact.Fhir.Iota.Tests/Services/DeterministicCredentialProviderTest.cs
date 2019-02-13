using System;
using System.Collections.Generic;
using System.Text;

namespace Pact.Fhir.Iota.Tests.Services
{
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Tests.Utils;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Curl;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;

  [TestClass]
  public class DeterministicCredentialProviderTest
  {
    [TestMethod]
    public async Task TestNoIndexIsUsedShouldUseFirstIndex()
    {
      var provider = new InMemoryDeterministicCredentialProvider(
        Seed.Random(),
        new InMemoryResourceTracker(),
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        IotaResourceProvider.Repository);

      await provider.CreateAsync();

      Assert.AreEqual(0, provider.CurrentIndex);
    }

    [TestMethod]
    public async Task TestFirstIndexHasBeenUsedShouldUseSecondIndex()
    {
      var provider = new InMemoryDeterministicCredentialProvider(
                       Seed.Random(),
                       new InMemoryResourceTracker(),
                       new IssSigningHelper(new Curl(), new Curl(), new Curl()),
                       new AddressGenerator(),
                       IotaResourceProvider.Repository) { CurrentIndex = 0 };

      await provider.CreateAsync();

      Assert.AreEqual(1, provider.CurrentIndex);
    }

    [TestMethod]
    public async Task TestIndexHasBeenWrittenFromAnotherSourceShouldSkipIndex()
    {
      var seed = Seed.Random();

      // Publish a message as if it was published from another app
      var tempProvider = new InMemoryDeterministicCredentialProvider(
                       seed,
                       new InMemoryResourceTracker(),
                       new IssSigningHelper(new Curl(), new Curl(), new Curl()),
                       new AddressGenerator(),
                       IotaResourceProvider.Repository);

      var credentials = await tempProvider.CreateAsync();
      var channel = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, IotaResourceProvider.Repository).Create(
        Mode.Restricted,
        credentials.Seed,
        IotaFhirRepository.SecurityLevel,
        credentials.ChannelKey);

      var message = channel.CreateMessage(TryteString.FromAsciiString("Test"));
      await channel.PublishAsync(message, 14, 1);

      // Create credentials that should skip the first index
      var provider = new InMemoryDeterministicCredentialProvider(
                       seed,
                       new InMemoryResourceTracker(),
                       new IssSigningHelper(new Curl(), new Curl(), new Curl()),
                       new AddressGenerator(),
                       IotaResourceProvider.Repository);

      await provider.CreateAsync();

      Assert.AreEqual(1, provider.CurrentIndex);
    }

    [TestMethod]
    public async Task TestSeedImportSetsCurrentIndexCorrectly()
    {
      var seed = Seed.Random();

      // Publish a message as if it was published from another app
      var tempProvider = new InMemoryDeterministicCredentialProvider(
        seed,
        new InMemoryResourceTracker(),
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        IotaResourceProvider.Repository);

      var credentials = await tempProvider.CreateAsync();
      var channel = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, IotaResourceProvider.Repository).Create(
        Mode.Restricted,
        credentials.Seed,
        IotaFhirRepository.SecurityLevel,
        credentials.ChannelKey);

      var message = channel.CreateMessage(TryteString.FromAsciiString("Test"));
      await channel.PublishAsync(message, 14, 1);

      // Create credentials that should skip the first index
      var provider = new InMemoryDeterministicCredentialProvider(
        seed,
        new InMemoryResourceTracker(),
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        IotaResourceProvider.Repository);

      await provider.SyncAsync();

      Assert.AreEqual(1, provider.CurrentIndex);
    }
  }
}
