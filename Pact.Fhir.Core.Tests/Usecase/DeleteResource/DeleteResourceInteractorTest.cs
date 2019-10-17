namespace Pact.Fhir.Core.Tests.Usecase.DeleteResource
{
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase.DeleteResource;

  [TestClass]
  public class DeleteResourceInteractorTest
  {
    [TestMethod]
    public async Task TestDeletionIsSuccessfulShouldReturnSuccessCode()
    {
      var resource = FhirResourceProvider.Patient;
      resource.PopulateMetadata("123456789", "123456789");

      var fhirRepository = new InMemoryFhirRepository();
      fhirRepository.Resources.Add(resource);
      Assert.AreEqual(1, fhirRepository.Resources.Count);

      var interactor = new DeleteResourceInteractor(fhirRepository, new InMemorySearchRepository());
      await interactor.ExecuteAsync(new DeleteResourceRequest { ResourceId = "123456789" });

      Assert.AreEqual(0, fhirRepository.Resources.Count);
    }
  }
}