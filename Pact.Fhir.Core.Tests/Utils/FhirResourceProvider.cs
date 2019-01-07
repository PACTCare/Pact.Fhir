namespace Pact.Fhir.Core.Tests.Utils
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  public static class FhirResourceProvider
  {
    public static Patient GetPatient()
    {
      return new Patient
               {
                 Name = new List<HumanName>
                          {
                            new HumanName
                              {
                                Use = HumanName.NameUse.Official,
                                Prefix = new[] { "Mr" },
                                Given = new[] { "Max" },
                                Family = "Mustermann"
                              }
                          },
                 Identifier = new List<Identifier>
                                {
                                  new Identifier { System = "http://ns.electronichealth.net.au/id/hi/ihi/1.0", Value = "8003608166690503" }
                                }
               };
    }
  }
}