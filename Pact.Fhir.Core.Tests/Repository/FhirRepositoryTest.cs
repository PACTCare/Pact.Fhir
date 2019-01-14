namespace Pact.Fhir.Core.Tests.Repository
{
  using System;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Tests.Utils;

  [TestClass]
  public class FhirRepositoryTest
  {
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task TestGivenIdsAreInvalidShouldThrowException()
    {
        var repository = new InMemoryFhirRepository("&/%/&%%&%&$()==");
        var resource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);
    }

    [TestMethod]
    public async Task TestGivenIdsAreTooLongShouldBeCutToLength()
    {
      var id = "UFYOWALBQSJWNKIZBYQ9YFPZYSWHWVOAXQPXRCCIYYNSJ9FLAIRKEIOPR9RDFEFWDJZXVMH9WIB9SXPZM";
      var repository = new InMemoryFhirRepository(id);
      var resource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);

      Assert.AreEqual(id.Substring(0, 64), resource.Id);
      Assert.AreEqual(id.Substring(0, 64), resource.VersionId);
    }

    [TestMethod]
    public async Task TestGivenIdsAreValidAndShorterThanMaxLengthShouldAssignThem()
    {
      var id = "UFYOWALBQSJWNKIZBYQ9YFP";
      var repository = new InMemoryFhirRepository(id);
      var resource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);

      Assert.AreEqual(id.Length, resource.Id.Length);
      Assert.AreEqual(id, resource.Id);

      Assert.AreEqual(id.Length, resource.VersionId.Length);
      Assert.AreEqual(id, resource.VersionId);
    }
  }
}