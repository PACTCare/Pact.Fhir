namespace Pact.Fhir.Iota.Console
{
  using System;
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  public static class ObservationFactory
  {
    public static Observation FromMeasurement(decimal glucoseConcentration, DateTimeOffset measurementDate)
    {
      return new Observation
               {
                 Identifier =
                   new List<Identifier>
                     {
                       new Identifier
                         {
                           System = "http://www.bmc.nl/zorgportal/identifiers/observations", Value = "6323", Use = Identifier.IdentifierUse.Official
                         }
                     },
                 Status = ObservationStatus.Final,
                 Code = new CodeableConcept
                          {
                            Coding = new List<Coding>
                                       {
                                         new Coding { System = "http://loinc.org", Code = "15074-8", Display = "Glucose [Moles/volume] in Blood" }
                                       }
                          },
                 Issued = DateTimeOffset.UtcNow,
                 Value = new SimpleQuantity { Value = glucoseConcentration, Unit = "mmol/l", System = "http://unitsofmeasure.org", Code = "mmol/l" },
                 Effective = new Period { StartElement = new FhirDateTime(measurementDate) }
               };
    }
  }
}