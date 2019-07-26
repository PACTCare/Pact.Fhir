using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pact.Fhir.Core.Repository;
using Pact.Fhir.Core.Tests.Repository;
using Pact.Fhir.Core.Tests.Utils;
using Pact.Fhir.Iota.Services;
using Tangle.Net.Entity;
using Task = System.Threading.Tasks.Task;

namespace Pact.Fhir.Iota.Tests.Services
{
  [TestClass]
  public class ResourceImporterTest
  {
    [TestMethod]
    public async Task TestResourceCanBeImportedAndIsAddedToSearch()
    {
      var fhirRepository = new InMemoryFhirRepository();
      var resourceTracker = new InMemoryResourceTracker();
      var searchRepository = new InMemorySearchRepository();

      var resourceImporter = new ResourceImporter(searchRepository, fhirRepository, new RandomSeedManager(resourceTracker));

      var rootHash = Seed.Random().Value;
      var patient = FhirResourceProvider.Patient;
      patient.Id = rootHash.Substring(0, 64);

      fhirRepository.Resources.Add(patient);

      await resourceImporter.ImportResourceAccessAsync(rootHash, Seed.Random().Value);

      Assert.IsNotNull(await resourceTracker.GetEntryAsync(rootHash));
      Assert.AreEqual(1, (await searchRepository.FindResourcesByTypeAsync("Patient")).Count);
    }
  }
}
