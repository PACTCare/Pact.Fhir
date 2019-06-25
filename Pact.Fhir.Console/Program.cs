using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Pact.Fhir.Core.Usecase.CreateResource;
using RestSharp;
using Task = System.Threading.Tasks.Task;

namespace Pact.Fhir.Console
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      RunAsync().Wait();
      System.Console.ReadKey();
    }

    private static async Task RunAsync()
    {
      var patient = new Patient
      {
        Name = new List<HumanName>
        {
          new HumanName
            {Use = HumanName.NameUse.Official, Prefix = new[] {"Mr"}, Given = new[] {"Max"}, Family = "Mustermann"}
        },
        Identifier = new List<Identifier>
        {
          new Identifier {System = "http://ns.electronichealth.net.au/id/hi/ihi/1.0", Value = "8003608166690503"}
        }
      };

      var response = await DependencyResolver.CreateResourceInteractor.ExecuteAsync(new CreateResourceRequest
        {ResourceJson = patient.ToJson()});
      System.Console.WriteLine($"Created resource: {response.Resource.Id}");

      var resourceEntry = await DependencyResolver.ResourceTracker.GetEntryAsync(response.Resource.Id);
      System.Console.WriteLine(
        $"Root: {resourceEntry.Subscription.MessageRoot.Value} ChannelKey: {resourceEntry.Subscription.Key.Value}");

      var restClient = new RestClient("http://localhost:64264/");
      var request = new RestRequest("api/iota/import", Method.POST) { RequestFormat = DataFormat.Json };
      request.AddJsonBody(new { root = resourceEntry.Subscription.MessageRoot.Value, channelKey = resourceEntry.Subscription.Key.Value });
      await restClient.ExecuteTaskAsync(request);

      System.Console.WriteLine("Shared record!");
    }
  }
}