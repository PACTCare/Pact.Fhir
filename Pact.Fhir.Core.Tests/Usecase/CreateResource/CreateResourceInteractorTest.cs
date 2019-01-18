namespace Pact.Fhir.Core.Tests.Usecase.CreateResource
{
  using System;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Moq;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services;
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

      var interactor = new CreateResourceInteractor(fhirRepository, new FhirResourceParser(new FhirJsonParser()));
      var response = await interactor.ExecuteAsync(
                       new CreateResourceRequest { ResourceJson = new FhirJsonSerializer().SerializeToString(resource), ResourceType = "Patient" });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(response.LogicalId, response.VersionId);
      Assert.AreEqual("SOMEFHIRCONFORMID1234", response.LogicalId);

      Assert.AreEqual(1, fhirRepository.Resources.Count);
    }

    [TestMethod]
    public async Task TestRepositoryThrowsExceptionShouldReturnErrorCode()
    {
      var repositoryMock = new Mock<IFhirRepository>();
      repositoryMock.Setup(r => r.CreateResourceAsync(It.IsAny<DomainResource>())).ThrowsAsync(new Exception("Catch me if you can"));
      var interactor = new CreateResourceInteractor(repositoryMock.Object, new FhirResourceParser(new FhirJsonParser()));

      var response = await interactor.ExecuteAsync(
                       new CreateResourceRequest
                         {
                           ResourceJson = new FhirJsonSerializer().SerializeToString(FhirResourceProvider.Patient), ResourceType = "Patient"
                         });

      Assert.AreEqual(ResponseCode.Failure, response.Code);
    }

    [DataRow("", "SomeUnsupportedResource", ResponseCode.UnsupportedResource)]
    [DataRow("Patient", "{\"resourceType\":\"afafasf\"}", ResponseCode.UnprocessableEntity)]
    [DataTestMethod]
    public async Task TestFhirResourceCanNotBeDeserializedShouldReturnErrorCode(string type, string data, ResponseCode expectedCode)
    {

      var interactor = new CreateResourceInteractor(new InMemoryFhirRepository(), new FhirResourceParser(new FhirJsonParser()));
      var response = await interactor.ExecuteAsync(
                       new CreateResourceRequest { ResourceJson = data, ResourceType = type});

      Assert.AreEqual(expectedCode, response.Code);
    }
  }
}