namespace Pact.Fhir.Core.Tests.Usecase.CreateResource
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.CreateResource;

  [TestClass]
  public class CreateResourceInteractorTest
  {
    [TestMethod]
    public void TestDomainResourceCanBeCreated()
    {
      var fhirRepository = new InMemoryFhirRepository();
      var resource = FhirResourceProvider.GetPatient();

      var interactor = new CreateResourceInteractor(fhirRepository);
      var response = interactor.Execute(new CreateResourceRequest { Resource = resource });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(1, fhirRepository.Resources.Count);
      Assert.IsTrue(resource.IsExactly(fhirRepository.Resources[0]));
    }
  }
}