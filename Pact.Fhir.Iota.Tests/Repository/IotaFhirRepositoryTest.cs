namespace Pact.Fhir.Iota.Tests.Repository
{
  using System;
  using System.Text.RegularExpressions;

  using Hl7.Fhir.Model;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Tests.Services;
  using Pact.Fhir.Iota.Tests.Utils;

  using Tangle.Net.Utils;

  using Task = System.Threading.Tasks.Task;

  [TestClass]
  public class IotaFhirRepositoryTest
  {
    [TestMethod]
    public async Task TestResourceCreationOnTangleShouldAssignHashesAsIds()
    {
      var resourceTracker = new InMemoryResourceTracker();
      var repository = new IotaFhirRepository(IotaResourceProvider.Repository, new FhirJsonTryteSerializer(), resourceTracker);
      var resource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);

      Assert.AreEqual(1, Regex.Matches(resource.Id, Id.PATTERN).Count);
      Assert.AreEqual(resource.Id, resource.VersionId);
      Assert.AreEqual(resource.VersionId, resource.Meta.VersionId);

      Assert.AreEqual(DateTime.UtcNow.Day, resource.Meta.LastUpdated?.DateTime.Day);
      Assert.AreEqual(DateTime.UtcNow.Month, resource.Meta.LastUpdated?.DateTime.Month);
      Assert.AreEqual(DateTime.UtcNow.Year, resource.Meta.LastUpdated?.DateTime.Year);

      Assert.IsTrue(InputValidator.IsTrytes(resource.Id));
      Assert.AreEqual(1, resourceTracker.Entries.Count);
    }
  }
}