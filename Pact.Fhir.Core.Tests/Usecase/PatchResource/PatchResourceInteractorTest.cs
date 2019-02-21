namespace Pact.Fhir.Core.Tests.Usecase.PatchResource
{
  using System;
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Moq;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services;
  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.PatchResource;

  using Task = System.Threading.Tasks.Task;

  [TestClass]
  public class PatchResourceInteractorTest
  {
    [TestMethod]
    public async Task TestExceptionIsThrownShouldReturnErrorCode()
    {
      var repository = new Mock<IFhirRepository>();
      repository.Setup(r => r.ReadResourceAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

      var interactor = new PatchResourceInteractor(repository.Object, null);
      var result = await interactor.ExecuteAsync(new PatchResourceRequest { ResourceId = "7812456" });

      Assert.AreEqual(ResponseCode.Failure, result.Code);
    }

    [TestMethod]
    public async Task TestResourceDoesExistShouldApplyPatch()
    {
      var repository = new InMemoryFhirRepository();
      var resource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);

      var patchedResource = new Patient();
      resource.CopyTo(patchedResource);
      patchedResource.BirthDate = "01.01.1111";

      var patchApplier = new Mock<IPatchApplier>();
      patchApplier.Setup(p => p.ApplyTo(It.IsAny<Resource>(), It.IsAny<List<PatchOperation>>())).Returns(patchedResource);

      var interactor = new PatchResourceInteractor(repository, patchApplier.Object);
      var response = await interactor.ExecuteAsync(
                     new PatchResourceRequest
                       {
                         ResourceId = resource.Id, Payload = "[{ \"op\": \"test\", \"path\": \"/a/b/c\", \"value\": \"foo\" }]"
                     });


      var savedResource = await repository.ReadResourceAsync(resource.Id);

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual("01.01.1111", ((Patient)response.Resource).BirthDate);
      Assert.AreEqual("01.01.1111", ((Patient)savedResource).BirthDate);
    }

    [TestMethod]
    public async Task TestResourceDoesNotExistShouldReturnErrorCode()
    {
      var repository = new Mock<IFhirRepository>();
      repository.Setup(r => r.ReadResourceAsync(It.IsAny<string>())).ThrowsAsync(new ResourceNotFoundException("7812456"));

      var interactor = new PatchResourceInteractor(repository.Object, null);
      var result = await interactor.ExecuteAsync(new PatchResourceRequest { ResourceId = "7812456" });

      Assert.AreEqual(ResponseCode.ResourceNotFound, result.Code);
    }
  }
}