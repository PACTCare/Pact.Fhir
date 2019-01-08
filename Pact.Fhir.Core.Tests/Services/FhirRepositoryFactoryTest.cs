namespace Pact.Fhir.Core.Tests.Services
{
  using System;
  using System.IO;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Services;
  using Pact.Fhir.Core.Tests.Repository;

  [TestClass]
  [Ignore("Deactivated until refactored with DI")]
  public class FhirRepositoryFactoryTest
  {
    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void TestAssemblyCanNotBeFoundShouldThrowException()
    {
      var repository = FhirRepositoryFactory.Create("Pact.Fhir.Core.Tests", "SomeRepository");
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void TestInstanceCanNotBeFoundShouldThrowException()
    {
      var repository = FhirRepositoryFactory.Create("Pact.Fhir.Core.Tests.dll", "SomeRepository");
    }

    [TestMethod]
    public void TestRepositoryExistsShouldCreateInstance()
    {
      var repository = FhirRepositoryFactory.Create(
        System.Reflection.Assembly.GetExecutingAssembly().Location,
        typeof(InMemoryFhirRepository).FullName);
      Assert.IsInstanceOfType(repository, typeof(InMemoryFhirRepository));
    }
  }
}