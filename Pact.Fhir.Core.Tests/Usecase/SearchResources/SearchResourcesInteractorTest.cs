namespace Pact.Fhir.Core.Tests.Usecase.SearchResources
{
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase.SearchResources;

  using Task = System.Threading.Tasks.Task;

  [TestClass]
  public class SearchResourcesInteractorTest
  {
    [TestMethod]
    public async Task TestNoResourcesAreFoundShouldReturnEmptyBundle()
    {
      var interactor = new SearchResourcesInteractor(new InMemoryFhirRepository(), new InMemorySearchRepository());
      var result = await interactor.ExecuteAsync(new SearchResourcesRequest { ResourceType = "Patient" });

      Assert.AreEqual(0, ((Bundle)result.Resource).Entry.Count);
    }

    [TestMethod]
    public async Task TestResourceHasReferenceShouldReturnResource()
    {
      var searchRepository = new InMemorySearchRepository();
      await searchRepository.AddResourceAsync(FhirResourceProvider.Observation);
      await searchRepository.AddResourceAsync(new Observation { Id = "4354363", Subject = new ResourceReference("did:iota:765658556") });

      var interactor = new SearchResourcesInteractor(new InMemoryFhirRepository(), searchRepository);
      var result = await interactor.ExecuteAsync(new SearchResourcesRequest { ResourceType = "Observation", Parameters = "_reference=did:iota:1234567890" });

      Assert.AreEqual(1, ((Bundle)result.Resource).Entry.Count);
    }

    [TestMethod]
    public async Task TestSearchByIdReturnsRightResource()
    {
      var searchRepository = new InMemorySearchRepository();
      await searchRepository.AddResourceAsync(FhirResourceProvider.Observation);
      await searchRepository.AddResourceAsync(new Observation { Id = "4354363", Subject = new ResourceReference("did:iota:765658556") });

      var interactor = new SearchResourcesInteractor(new InMemoryFhirRepository(), searchRepository);
      var result = await interactor.ExecuteAsync(new SearchResourcesRequest { ResourceType = "Observation", Parameters = "_id=4354363" });

      Assert.AreEqual(1, ((Bundle)result.Resource).Entry.Count);
      Assert.AreEqual("4354363", ((Bundle)result.Resource).Entry.First().Resource.Id);
    }

    [TestMethod]
    public async Task TestSearchByTagReturnsRightResource()
    {
      var searchRepository = new InMemorySearchRepository();
      await searchRepository.AddResourceAsync(FhirResourceProvider.Observation);
      await searchRepository.AddResourceAsync(new Observation { Id = "4354363", Subject = new ResourceReference("did:iota:765658556"), Meta = new Meta {Tag = new List<Coding> {new Coding("Test", "Unit")}}});

      var interactor = new SearchResourcesInteractor(new InMemoryFhirRepository(), searchRepository);
      var result = await interactor.ExecuteAsync(new SearchResourcesRequest { ResourceType = "Observation", Parameters = "_tag=Test|" });

      Assert.AreEqual(1, ((Bundle)result.Resource).Entry.Count);
      Assert.AreEqual("4354363", ((Bundle)result.Resource).Entry.First().Resource.Id);
    }
  }
}