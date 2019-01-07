﻿namespace Pact.Fhir.Iota.Tests.Repository
{
  using System;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Fhir.Core.Tests.Utils;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Tests.Utils;

  using Tangle.Net.Utils;

  [TestClass]
  public class IotaFhirRepositoryTest
  {
    [TestMethod]
    public async Task TestResourceCreationOnTangleShouldAssignHashesAsIds()
    {
      var repository = new IotaFhirRepository(IotaResourceProvider.Repository, new FhirJsonTryteSerializer());
      var resource = await repository.CreateResourceAsync(FhirResourceProvider.Patient);

      Assert.AreEqual(resource.Id, resource.VersionId);
      Assert.IsTrue(InputValidator.IsTrytes(resource.Id));

      Assert.AreEqual(resource.VersionId, resource.Meta.VersionId);
      Assert.AreEqual(DateTime.UtcNow.Day, resource.Meta.LastUpdated?.DateTime.Day);
      Assert.AreEqual(DateTime.UtcNow.Month, resource.Meta.LastUpdated?.DateTime.Month);
      Assert.AreEqual(DateTime.UtcNow.Year, resource.Meta.LastUpdated?.DateTime.Year);
    }
  }
}