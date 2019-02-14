namespace Pact.Fhir.Iota.Console
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.UpdateResource;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Services;

  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public class Program
  {
    internal static void Main(string[] args)
    {
      var iotaRepository = new RestIotaRepository(
        new FallbackIotaClient(
          new List<string>
            {
              "http://node04.iotatoken.nl:14265",
              "http://node05.iotatoken.nl:16265",
              "https://nodes.thetangle.org:443",
              "http://iota1.heidger.eu:14265",
              "https://nodes.iota.cafe:443",
              "https://potato.iotasalad.org:14265",
              "https://durian.iotasalad.org:14265",
              "https://turnip.iotasalad.org:14265",
              "https://nodes.iota.fm:443",
              "https://tuna.iotasalad.org:14265",
              "https://iotanode2.jlld.at:443",
              "https://node.iota.moe:443",
              "https://wallet1.iota.town:443",
              "https://wallet2.iota.town:443",
              "http://node03.iotatoken.nl:15265",
              "https://node.iota-tangle.io:14265",
              "https://pow4.iota.community:443",
              "https://dyn.tangle-nodes.com:443",
              "https://pow5.iota.community:443",
            },
          5000),
        new PoWSrvService());

      var fhirRepository = new IotaFhirRepository(
        iotaRepository,
        new FhirJsonTryteSerializer(),
        new InMemoryResourceTracker(),
        new RandomChannelCredentialProvider());

      var createInteractor = new CreateResourceInteractor(fhirRepository, new FhirJsonParser());
      var updateInteractor = new UpdateResourceInteractor(fhirRepository, new FhirJsonParser());

      var observations = new List<Observation>
                           {
                             ObservationFactory.FromMeasurement(6.4m, new DateTime(2019, 2, 2, 0, 0, 0)),
                             ObservationFactory.FromMeasurement(5.3m, new DateTime(2019, 2, 2, 1, 0, 0)),
                             ObservationFactory.FromMeasurement(5.4m, new DateTime(2019, 2, 2, 2, 0, 0)),
                             ObservationFactory.FromMeasurement(5.5m, new DateTime(2019, 2, 2, 3, 0, 0)),
                             ObservationFactory.FromMeasurement(5.6m, new DateTime(2019, 2, 2, 4, 0, 0)),
                             ObservationFactory.FromMeasurement(5.4m, new DateTime(2019, 2, 2, 5, 0, 0)),
                             ObservationFactory.FromMeasurement(5.3m, new DateTime(2019, 2, 2, 6, 0, 0)),
                             ObservationFactory.FromMeasurement(5.2m, new DateTime(2019, 2, 2, 7, 0, 0)),
                             ObservationFactory.FromMeasurement(7.1m, new DateTime(2019, 2, 2, 8, 0, 0)),
                             ObservationFactory.FromMeasurement(8.3m, new DateTime(2019, 2, 2, 9, 0, 0)),
                             ObservationFactory.FromMeasurement(9.2m, new DateTime(2019, 2, 2, 10, 0, 0)),
                             ObservationFactory.FromMeasurement(8.1m, new DateTime(2019, 2, 2, 11, 0, 0)),
                             ObservationFactory.FromMeasurement(7.3m, new DateTime(2019, 2, 2, 12, 0, 0)),
                             ObservationFactory.FromMeasurement(6.1m, new DateTime(2019, 2, 2, 13, 0, 0)),
                             ObservationFactory.FromMeasurement(6.4m, new DateTime(2019, 2, 2, 14, 0, 0)),
                             ObservationFactory.FromMeasurement(6.7m, new DateTime(2019, 2, 2, 15, 0, 0)),
                             ObservationFactory.FromMeasurement(6.5m, new DateTime(2019, 2, 2, 16, 0, 0)),
                             ObservationFactory.FromMeasurement(5.1m, new DateTime(2019, 2, 2, 17, 0, 0)),
                             ObservationFactory.FromMeasurement(4.3m, new DateTime(2019, 2, 2, 18, 0, 0)),
                             ObservationFactory.FromMeasurement(5.2m, new DateTime(2019, 2, 2, 19, 0, 0)),
                             ObservationFactory.FromMeasurement(6.5m, new DateTime(2019, 2, 2, 20, 0, 0)),
                             ObservationFactory.FromMeasurement(7.3m, new DateTime(2019, 2, 2, 21, 0, 0)),
                             ObservationFactory.FromMeasurement(7.1m, new DateTime(2019, 2, 2, 22, 0, 0)),
                             ObservationFactory.FromMeasurement(6.7m, new DateTime(2019, 2, 2, 23, 0, 0)),
                             ObservationFactory.FromMeasurement(6.4m, new DateTime(2019, 2, 3, 0, 0, 0)),
                             ObservationFactory.FromMeasurement(5.3m, new DateTime(2019, 2, 3, 1, 0, 0)),
                             ObservationFactory.FromMeasurement(5.4m, new DateTime(2019, 2, 3, 2, 0, 0)),
                             ObservationFactory.FromMeasurement(5.5m, new DateTime(2019, 2, 3, 3, 0, 0)),
                             ObservationFactory.FromMeasurement(5.6m, new DateTime(2019, 2, 3, 4, 0, 0)),
                             ObservationFactory.FromMeasurement(5.4m, new DateTime(2019, 2, 3, 5, 0, 0)),
                             ObservationFactory.FromMeasurement(5.3m, new DateTime(2019, 2, 3, 6, 0, 0)),
                             ObservationFactory.FromMeasurement(5.2m, new DateTime(2019, 2, 3, 7, 0, 0)),
                             ObservationFactory.FromMeasurement(7.1m, new DateTime(2019, 2, 3, 8, 0, 0)),
                             ObservationFactory.FromMeasurement(8.3m, new DateTime(2019, 2, 3, 9, 0, 0)),
                             ObservationFactory.FromMeasurement(9.2m, new DateTime(2019, 2, 3, 10, 0, 0)),
                             ObservationFactory.FromMeasurement(8.1m, new DateTime(2019, 2, 3, 11, 0, 0)),
                             ObservationFactory.FromMeasurement(7.3m, new DateTime(2019, 2, 3, 12, 0, 0)),
                             ObservationFactory.FromMeasurement(6.1m, new DateTime(2019, 2, 3, 13, 0, 0)),
                             ObservationFactory.FromMeasurement(6.4m, new DateTime(2019, 2, 3, 14, 0, 0)),
                             ObservationFactory.FromMeasurement(6.7m, new DateTime(2019, 2, 3, 15, 0, 0)),
                             ObservationFactory.FromMeasurement(6.5m, new DateTime(2019, 2, 3, 16, 0, 0)),
                             ObservationFactory.FromMeasurement(5.1m, new DateTime(2019, 2, 3, 17, 0, 0)),
                             ObservationFactory.FromMeasurement(4.3m, new DateTime(2019, 2, 3, 18, 0, 0)),
                             ObservationFactory.FromMeasurement(5.2m, new DateTime(2019, 2, 3, 19, 0, 0)),
                             ObservationFactory.FromMeasurement(6.5m, new DateTime(2019, 2, 3, 20, 0, 0)),
                             ObservationFactory.FromMeasurement(7.3m, new DateTime(2019, 2, 3, 21, 0, 0)),
                             ObservationFactory.FromMeasurement(7.1m, new DateTime(2019, 2, 3, 22, 0, 0)),
                             ObservationFactory.FromMeasurement(6.7m, new DateTime(2019, 2, 3, 23, 0, 0)),
                           };                                                               

      var serializer = new FhirJsonSerializer();

      var result = createInteractor.ExecuteAsync(new CreateResourceRequest { ResourceJson = serializer.SerializeToString(observations.First()) })
        .Result;

      var resource = (Observation)result.Resource;
      for (var i = 1; i < observations.Count; i++)
      {
        ((SimpleQuantity)resource.Value).Value = ((SimpleQuantity)observations[i].Value).Value;
        ((Period)resource.Effective).Start = ((Period)observations[i].Effective).Start;

        var a = updateInteractor
          .ExecuteAsync(new UpdateResourceRequest { ResourceId = resource.Id, ResourceJson = serializer.SerializeToString(resource) }).Result;

        Thread.Sleep(3000);
      }

      Console.ReadKey();
    }
  }
}