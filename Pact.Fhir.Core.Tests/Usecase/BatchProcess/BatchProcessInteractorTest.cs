namespace Pact.Fhir.Core.Tests.Usecase.BatchProcess
{
  using System;
  using System.Collections.Generic;
  using System.Net;

  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Moq;

  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.BatchProcess;

  using Task = System.Threading.Tasks.Task;

  [TestClass]
  public class BatchProcessInteractorTest
  {
    [TestMethod]
    public async Task TestOperationIsNotSupportedShouldIncludeErrorStatus()
    {
      var repositoryMock = new Mock<IFhirRepository>();
      repositoryMock.Setup(r => r.DeleteResourceAsync(It.IsAny<string>())).ThrowsAsync(new UnsupportedOperationException("DELETE"));

      var interactor = new BatchProcessInteractor(repositoryMock.Object);
      var response = await interactor.ExecuteAsync(
                       new BatchProcessRequest
                         {
                           Bundle = new Bundle
                                      {
                                        Entry = new List<Bundle.EntryComponent>
                                                  {
                                                    new Bundle.EntryComponent
                                                      {
                                                        Resource = FhirResourceProvider.Patient,
                                                        Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.DELETE }
                                                      }
                                                  }
                                      }
                         });

      var responseBundle = (Bundle)response.Resource;

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(1, responseBundle.Entry.Count);
      Assert.AreEqual(HttpStatusCode.MethodNotAllowed.ToString(), responseBundle.Entry[0].Response.Status);
    }

    [TestMethod]
    public async Task TestResourceNotFoundShouldIncludeErrorStatus()
    {
      var repositoryMock = new Mock<IFhirRepository>();
      repositoryMock.Setup(r => r.ReadResourceAsync(It.IsAny<string>())).ThrowsAsync(new ResourceNotFoundException("id"));

      var interactor = new BatchProcessInteractor(repositoryMock.Object);
      var response = await interactor.ExecuteAsync(
                       new BatchProcessRequest
                         {
                           Bundle = new Bundle
                                      {
                                        Entry = new List<Bundle.EntryComponent>
                                                  {
                                                    new Bundle.EntryComponent
                                                      {
                                                        Resource = FhirResourceProvider.Patient,
                                                        Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.GET }
                                                      }
                                                  }
                                      }
                         });

      var responseBundle = (Bundle)response.Resource;

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(1, responseBundle.Entry.Count);
      Assert.AreEqual(HttpStatusCode.NotFound.ToString(), responseBundle.Entry[0].Response.Status);
    }

    [TestMethod]
    public async Task TestRepositoryThrowsUnknownExceptionShouldIncludeErrorStatus()
    {
      var repositoryMock = new Mock<IFhirRepository>();
      repositoryMock.Setup(r => r.ReadResourceAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

      var interactor = new BatchProcessInteractor(repositoryMock.Object);
      var response = await interactor.ExecuteAsync(
                       new BatchProcessRequest
                         {
                           Bundle = new Bundle
                                      {
                                        Entry = new List<Bundle.EntryComponent>
                                                  {
                                                    new Bundle.EntryComponent
                                                      {
                                                        Resource = FhirResourceProvider.Patient,
                                                        Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.GET }
                                                      }
                                                  }
                                      }
                         });

      var responseBundle = (Bundle)response.Resource;

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(1, responseBundle.Entry.Count);
      Assert.AreEqual(HttpStatusCode.InternalServerError.ToString(), responseBundle.Entry[0].Response.Status);
    }

    [TestMethod]
    public async Task TestSuccessfulBatchShouldReturnSuccessOnAllEntries()
    {
      var resource = FhirResourceProvider.Patient;
      resource.Id = "SOMEFHIRCONFORMID";

      var repository = new InMemoryFhirRepository();
      repository.Resources.Add(resource);

      var interactor = new BatchProcessInteractor(repository);
      var response = await interactor.ExecuteAsync(
                       new BatchProcessRequest
                         {
                           Bundle = new Bundle
                                      {
                                        Entry = new List<Bundle.EntryComponent>
                                                  {
                                                    new Bundle.EntryComponent
                                                      {
                                                        Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.GET, ElementId = "SOMEFHIRCONFORMID"}, 
                                                      },
                                                    new Bundle.EntryComponent
                                                      {
                                                        Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.DELETE, ElementId = "SOMEFHIRCONFORMID"},
                                                      },
                                                  }
                                      }
                         });

      var responseBundle = (Bundle)response.Resource;

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(2, responseBundle.Entry.Count);
      Assert.IsNotNull(responseBundle.Entry[0].Resource);
      Assert.AreEqual(0, repository.Resources.Count);
    }
  }
}