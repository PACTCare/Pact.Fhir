namespace Pact.Fhir.Core.Tests.Usecase.ReadResourceHistory
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Moq;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.ReadResourceHistory;

  using Task = System.Threading.Tasks.Task;

  [TestClass]
  public class ReadResourceHistoryInteractorTest
  {
    [TestMethod]
    public async Task TestRepositoryThrowsExceptionShouldReturnErrorCode()
    {
      var repository = new Mock<IFhirRepository>();
      repository.Setup(r => r.ReadResourceHistoryAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

      var interactor = new ReadResourceHistoryInteractor(repository.Object);
      var response = await interactor.ExecuteAsync(new ReadResourceHistoryRequest { ResourceId = "kasfasagdssg", ResourceType = "Patient" });

      Assert.AreEqual(ResponseCode.Failure, response.Code);
    }

    [TestMethod]
    public async Task TestRepositoryReturnsNoResourcesShouldReturnErrorCode()
    {
      var repository = new Mock<IFhirRepository>();
      repository.Setup(r => r.ReadResourceHistoryAsync(It.IsAny<string>())).ReturnsAsync(new List<Resource>());

      var interactor = new ReadResourceHistoryInteractor(repository.Object);
      var response = await interactor.ExecuteAsync(new ReadResourceHistoryRequest { ResourceId = "kasfasagdssg", ResourceType = "Patient" });

      Assert.AreEqual(ResponseCode.ResourceNotFound, response.Code);
    }

    [TestMethod]
    public async Task TestResourceDoesNotExistShouldReturnErrorCode()
    {
      var interactor = new ReadResourceHistoryInteractor(new InMemoryFhirRepository());
      var response = await interactor.ExecuteAsync(new ReadResourceHistoryRequest { ResourceId = "kasfasagdssg", ResourceType = "Patient" });

      Assert.AreEqual(ResponseCode.ResourceNotFound, response.Code);
    }

    [TestMethod]
    public async Task TestResourceExistsShouldReturnResourceAndSuccess()
    {
      var resource = FhirResourceProvider.Patient;
      resource.Id = "SOMEFHIRCONFORMID";

      var repository = new InMemoryFhirRepository();
      repository.Resources.Add(resource);

      var interactor = new ReadResourceHistoryInteractor(repository);
      var response = await interactor.ExecuteAsync(new ReadResourceHistoryRequest { ResourceId = "SOMEFHIRCONFORMID", ResourceType = "Patient" });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.IsTrue(((Bundle)response.Resource).Entry.First().Resource.IsExactly(resource));
    }

    [TestMethod]
    public async Task TestResourceTypeMismatchShouldReturnErrorCode()
    {
      var resource = FhirResourceProvider.Patient;
      resource.Id = "SOMEFHIRCONFORMID";

      var repository = new InMemoryFhirRepository();
      repository.Resources.Add(resource);

      var interactor = new ReadResourceHistoryInteractor(repository);
      var response = await interactor.ExecuteAsync(new ReadResourceHistoryRequest { ResourceId = "SOMEFHIRCONFORMID", ResourceType = "Observation" });

      Assert.AreEqual(ResponseCode.ResourceNotFound, response.Code);
    }
  }
}