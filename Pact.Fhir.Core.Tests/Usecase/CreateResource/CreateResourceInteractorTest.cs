namespace Pact.Fhir.Core.Tests.Usecase.CreateResource
{
  using System;

  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Moq;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.CreateResource;

  using Task = System.Threading.Tasks.Task;

  [TestClass]
  public class CreateResourceInteractorTest
  {
    [TestMethod]
    public async Task TestDomainResourceCanBeCreated()
    {
      var fhirRepository = new InMemoryFhirRepository();
      var resource = FhirResourceProvider.Patient;
      resource.Id = "SomeValueThatShouldBeIgnored";

      var interactor = new CreateResourceInteractor(fhirRepository);
      var response = await interactor.ExecuteAsync(new CreateResourceRequest { Resource = resource });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(response.LogicalId, response.VersionId);
      Assert.AreEqual("SOMEFHIRCONFORMID1234", response.LogicalId);

      Assert.AreEqual(1, fhirRepository.Resources.Count);
      Assert.IsTrue(resource.IsExactly(fhirRepository.Resources[0]));
    }

    [TestMethod]
    public async Task TestRepositoryThrowsExceptionShouldReturnErrorCode()
    {
      var repositoryMock = new Mock<IFhirRepository>();
      repositoryMock.Setup(r => r.CreateResourceAsync(It.IsAny<DomainResource>())).ThrowsAsync(new Exception("Catch me if you can"));
      var interactor = new CreateResourceInteractor(repositoryMock.Object);

      var response = await interactor.ExecuteAsync(new CreateResourceRequest { Resource = FhirResourceProvider.Patient });
      Assert.AreEqual(ResponseCode.Failure, response.Code);
    }
  }
}