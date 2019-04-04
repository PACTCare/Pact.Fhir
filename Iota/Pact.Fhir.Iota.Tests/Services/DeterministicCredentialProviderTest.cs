﻿using System;
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
        new InMemoryResourceTracker(),
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        IotaResourceProvider.Repository);

      await provider.CreateAsync(Seed.Random());

      Assert.AreEqual(0, provider.CurrentIndex);
    }

    [TestMethod]
    public async Task TestFirstIndexHasBeenUsedShouldUseSecondIndex()
    {
      var provider = new InMemoryDeterministicCredentialProvider(
                       new InMemoryResourceTracker(),
                       new IssSigningHelper(new Curl(), new Curl(), new Curl()),
                       new AddressGenerator(),
                       IotaResourceProvider.Repository) { CurrentIndex = 0 };

      await provider.CreateAsync(Seed.Random());

      Assert.AreEqual(1, provider.CurrentIndex);
    }

    [TestMethod]
    public async Task TestIndexHasBeenWrittenFromAnotherSourceShouldSkipIndex()
    {
      var seed = Seed.Random();

      // Publish a message as if it was published from another app
      var tempProvider = new InMemoryDeterministicCredentialProvider(
                       new InMemoryResourceTracker(),
                       new IssSigningHelper(new Curl(), new Curl(), new Curl()),
                       new AddressGenerator(),
                       IotaResourceProvider.Repository);

      var credentials = await tempProvider.CreateAsync(seed);
      var channel = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, IotaResourceProvider.Repository).Create(
        Mode.Restricted,
        credentials.Seed,
        IotaFhirRepository.SecurityLevel,
        credentials.ChannelKey);

      var message = channel.CreateMessage(TryteString.FromAsciiString("Test"));
      await channel.PublishAsync(message, 14, 1);

      // Create credentials that should skip the first index
      var provider = new InMemoryDeterministicCredentialProvider(
                       new InMemoryResourceTracker(),
                       new IssSigningHelper(new Curl(), new Curl(), new Curl()),
                       new AddressGenerator(),
                       IotaResourceProvider.Repository);

      await provider.CreateAsync(seed);

      Assert.AreEqual(1, provider.CurrentIndex);
    }

    [TestMethod]
    public async Task TestSeedImportSetsCurrentIndexCorrectlyAndFiresEventForFoundSubscription()
    {
      var seed = Seed.Random();

      // Publish a message as if it was published from another app
      var tempProvider = new InMemoryDeterministicCredentialProvider(
        new InMemoryResourceTracker(),
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        IotaResourceProvider.Repository);

      var credentials = await tempProvider.CreateAsync(seed);
      var channel = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, IotaResourceProvider.Repository).Create(
        Mode.Restricted,
        credentials.Seed,
        IotaFhirRepository.SecurityLevel,
        credentials.ChannelKey);

      var message = channel.CreateMessage(TryteString.FromAsciiString("Test"));
      await channel.PublishAsync(message, 14, 1);

      var credentials2 = await tempProvider.CreateAsync(seed);
      var channel2 = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, IotaResourceProvider.Repository).Create(
        Mode.Restricted,
        credentials2.Seed,
        IotaFhirRepository.SecurityLevel,
        credentials2.ChannelKey);

      var message2 = channel2.CreateMessage(TryteString.FromAsciiString("Test"));
      await channel2.PublishAsync(message2, 14, 1);

      // Create credentials that should skip the first index
      var provider = new InMemoryDeterministicCredentialProvider(
        new InMemoryResourceTracker(),
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        IotaResourceProvider.Repository);

      var subFoundEventCounter = 0;
      provider.SubscriptionFound += (sender, args) => { subFoundEventCounter = subFoundEventCounter + 1; };

      await provider.SyncAsync(seed);

      Assert.AreEqual(2, subFoundEventCounter);
      Assert.AreEqual(2, provider.CurrentIndex);
    }
  }
}
