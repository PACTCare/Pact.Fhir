using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Serialization;
using Pact.Fhir.Core.SqlLite.Repository;
using Pact.Fhir.Core.Usecase.CreateResource;
using Pact.Fhir.Core.Usecase.GetCapabilities;
using Pact.Fhir.Core.Usecase.ReadResource;
using Pact.Fhir.Core.Usecase.SearchResources;
using Pact.Fhir.Core.Usecase.ValidateResource;
using Pact.Fhir.Iota.Repository;
using Pact.Fhir.Iota.Serializer;
using Pact.Fhir.Iota.Services;
using Pact.Fhir.Iota.SqlLite.Encryption;
using Pact.Fhir.Iota.SqlLite.Services;
using Tangle.Net.Cryptography;
using Tangle.Net.Cryptography.Curl;
using Tangle.Net.Cryptography.Signing;
using Tangle.Net.Mam.Merkle;
using Tangle.Net.Mam.Services;
using Tangle.Net.ProofOfWork.Service;
using Tangle.Net.Repository.Client;

namespace Pact.Fhir.Console
{
  public static class DependencyResolver
  {
    static DependencyResolver()
    {
      var iotaRepository = new CachedIotaRestRepository(
        new FallbackIotaClient(new List<string> { "https://nodes.devnet.thetangle.org:443" }, 5000),
        new PoWSrvService());

      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var encryption = new RijndaelEncryption("somenicekey", "somenicesalt");
      var resourceTracker = new SqlLiteResourceTracker(channelFactory, subscriptionFactory, encryption);

      var fhirRepository = new IotaFhirRepository(
        iotaRepository,
        new FhirJsonTryteSerializer(),
        resourceTracker,
        new SqlLiteDeterministicSeedManager(
          resourceTracker,
          new IssSigningHelper(new Curl(), new Curl(), new Curl()),
          new AddressGenerator(),
          iotaRepository,
          encryption));

      var fhirParser = new FhirJsonParser();
      var searchRepository = new SqlLiteSearchRepository(fhirParser);

      CreateResourceInteractor = new CreateResourceInteractor(fhirRepository, fhirParser, searchRepository);
      ReadResourceInteractor = new ReadResourceInteractor(fhirRepository, searchRepository);
      ValidateResourceInteractor = new ValidateResourceInteractor(fhirRepository, fhirParser);
      SearchResourcesInteractor = new SearchResourcesInteractor(fhirRepository, searchRepository);
      ResourceTracker = resourceTracker;
    }

    public static CreateResourceInteractor CreateResourceInteractor { get; set; }
    public static ReadResourceInteractor ReadResourceInteractor { get; set; }
    public static ValidateResourceInteractor ValidateResourceInteractor { get; set; }
    public static SearchResourcesInteractor SearchResourcesInteractor { get; set; }
    public static IResourceTracker ResourceTracker { get; set; }
  }
}
