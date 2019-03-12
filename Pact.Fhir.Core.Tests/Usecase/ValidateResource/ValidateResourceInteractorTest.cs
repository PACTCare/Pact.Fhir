namespace Pact.Fhir.Core.Tests.Usecase.ValidateResource
{
  using System;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Tests.Repository;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.ValidateResource;

  using Task = System.Threading.Tasks.Task;

  [TestClass]
  public class ValidateResourceInteractorTest
  {
    [TestMethod]
    public async Task TestJsonIsMalformedShouldReturnSuccessWithValidationError()
    {
      var interactor = new ValidateResourceInteractor(new InMemoryFhirRepository(), new FhirJsonParser());
      var response = await interactor.ExecuteAsync(new ValidateResourceRequest { ResourceJson = "{  \"resourceType\":\"Patient\", \"id\":\"RKSMLXYC9Q9XCPPTOHSHH9KZSPQNJRSDMTYTLQM9HOKKHECZYJFKPOREHNCOWPRD\",\"meta\" { \"versionId\":\"RKSMLXYC9Q9XCPPTOHSHH9KZSPQNJRSDMTYTLQM9HOKKHECZYJFKPOREHNCOWPRD\",\"lastUpdated\":\"2019-01-18T13:20:21.6148332+00:00\"},\"identifier\":[  {  \"system\":\"http://ns.electronichealth.net.au/id/hi/ihi/1.0\",\"value\":\"8003608166690503\"}],\"namee\":[  {   \"use\":\"official\",\"family\":\"Mustermann\",}]}" });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(1, response.ValidationResult.Count);
    }

    [TestMethod]
    public async Task TestResourceIsValidShouldReturnSuccessWithoutValidationErrors()
    {
      var interactor = new ValidateResourceInteractor(new InMemoryFhirRepository(), new FhirJsonParser());
      var response = await interactor.ExecuteAsync(new ValidateResourceRequest { ResourceJson = "{\"resourceType\":\"Patient\",\"id\":\"RKSMLXYC9Q9XCPPTOHSHH9KZSPQNJRSDMTYTLQM9HOKKHECZYJFKPOREHNCOWPRD\",\"meta\":{\"versionId\":\"RKSMLXYC9Q9XCPPTOHSHH9KZSPQNJRSDMTYTLQM9HOKKHECZYJFKPOREHNCOWPRD\",\"lastUpdated\":\"2019-01-18T13:20:21.6148332+00:00\"},\"identifier\":[{\"system\":\"http://ns.electronichealth.net.au/id/hi/ihi/1.0\",\"value\":\"8003608166690503\"}],\"name\":[{\"use\":\"official\",\"family\":\"Mustermann\"}]}" });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(0, response.ValidationResult.Count);
    }
  }
}