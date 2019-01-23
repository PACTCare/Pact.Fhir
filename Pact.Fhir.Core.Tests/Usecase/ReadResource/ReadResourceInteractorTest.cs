namespace Pact.Fhir.Core.Tests.Usecase.ReadResource
{
  using System;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Moq;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.ReadResource;

  [TestClass]
  public class ReadResourceInteractorTest
  {
    [TestMethod]
    public async Task TestRepositoryThrowsExceptionShouldReturnErrorCode()
    {
      var repository = new Mock<IFhirRepository>();
      repository.Setup(r => r.ReadResourceAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

      var interactor = new ReadResourceInteractor(repository.Object);
      var response = await interactor.ExecuteAsync(new ReadResourceRequest { ResourceId = "kasfasagdssg", ResourceType = "Patient" });

      Assert.AreEqual(ResponseCode.Failure, response.Code);
    }

    [TestMethod]
    public async Task TestResourceDoesNotExistShouldReturnErrorCode()
    {
      var interactor = new ReadResourceInteractor(new InMemoryFhirRepository());
      var response = await interactor.ExecuteAsync(new ReadResourceRequest { ResourceId = "kasfasagdssg", ResourceType = "Patient" });

      Assert.AreEqual(ResponseCode.ResourceNotFound, response.Code);
    }

    [TestMethod]
    public async Task TestResourceExistsShouldReturnResourceAndSuccess()
    {
      var resource = FhirResourceProvider.Patient;
      resource.Id = "SOMEFHIRCONFORMID";

      var repository = new InMemoryFhirRepository();
      repository.Resources.Add(resource);

      var interactor = new ReadResourceInteractor(repository);
      var response = await interactor.ExecuteAsync(new ReadResourceRequest { ResourceId = "SOMEFHIRCONFORMID", ResourceType = "Patient" });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.IsTrue(response.Resource.IsExactly(resource));
    }

    [TestMethod]
    public async Task TestResourceTypeMismatchShouldReturnErrorCode()
    {
      var resource = FhirResourceProvider.Patient;
      resource.Id = "SOMEFHIRCONFORMID";

      var repository = new InMemoryFhirRepository();
      repository.Resources.Add(resource);

      var interactor = new ReadResourceInteractor(repository);
      var response = await interactor.ExecuteAsync(new ReadResourceRequest { ResourceId = "SOMEFHIRCONFORMID", ResourceType = "Observation" });

      Assert.AreEqual(ResponseCode.ResourceNotFound, response.Code);
    }
  }
}