namespace Pact.Fhir.Core.Tests.Usecase.CreateResource
{
  using System;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.CreateResource;

  [TestClass]
  public class CreateResourceInteractorTest
  {
    [TestMethod]
    public async Task TestDomainResourceCanBeCreated()
    {
      var fhirRepository = new InMemoryFhirRepository();
      var resource = FhirResourceProvider.GetPatient();
      resource.Id = "SomeValueThatShouldBeIgnored";

      var interactor = new CreateResourceInteractor(fhirRepository);
      var response = await interactor.ExecuteAsync(new CreateResourceRequest { Resource = resource });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(response.LogicalId, response.VersionId);
      Assert.IsTrue(Guid.TryParse(response.LogicalId, out _));

      Assert.AreEqual(1, fhirRepository.Resources.Count);
      Assert.IsTrue(resource.IsExactly(fhirRepository.Resources[0]));
    }
  }
}