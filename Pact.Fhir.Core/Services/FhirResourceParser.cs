namespace Pact.Fhir.Core.Services
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Exception;

  public class FhirResourceParser
  {
    public FhirResourceParser(FhirJsonParser jsonParser)
    {
      this.JsonParser = jsonParser;
    }

    private FhirJsonParser JsonParser { get; }

    public DomainResource Parse(string resourceType, string resourceJson)
    {
      switch (resourceType)
      {
        case "Patient":
          return this.JsonParser.Parse<Patient>(resourceJson);
        case "Observation":
          return this.JsonParser.Parse<Observation>(resourceJson);
      }

      throw new UnsupportedResourceException(resourceType);
    }
  }
}