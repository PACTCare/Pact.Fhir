namespace Pact.Fhir.Core.Tests.Utils
{
  using Hl7.Fhir.Model;

  internal static class FhirResourceProvider
  {
    public static Patient GetPatient()
    {
      var patient = new Patient();

      var id = new Identifier { System = "http://hl7.org/fhir/sid/us-ssn", Value = "000-12-3456" };
      patient.Identifier.Add(id);

      var name = new HumanName().WithGiven("Christopher").WithGiven("C.H.").AndFamily("Parks");
      name.Prefix = new[] { "Mr." };
      name.Use = HumanName.NameUse.Official;

      var nickname = new HumanName { Use = HumanName.NameUse.Nickname };
      nickname.GivenElement.Add(new FhirString("Chris"));

      patient.Gender = AdministrativeGender.Male;
      patient.Name.Add(name);
      patient.Name.Add(nickname);

      patient.BirthDate = "1983-04-23";

      patient.Extension.Add(new Extension { Url = "http://hl7.org/fhir/StructureDefinition/birthPlace", Value = new Address { City = "Seattle" } });
      patient.BirthDateElement.Extension.Add(
        new Extension("http://hl7.org/fhir/StructureDefinition/patient-birthTime", new FhirDateTime(1983, 4, 23, 7, 44)));

      var contact = new Patient.ContactComponent
                      {
                        Name = new HumanName { Given = new[] { "Susan" }, Family = "Parks" }, Gender = AdministrativeGender.Female
                      };
      contact.Relationship.Add(new CodeableConcept("http://hl7.org/fhir/v2/0131", "N"));
      contact.Telecom.Add(new ContactPoint(ContactPoint.ContactPointSystem.Phone, null, string.Empty));
      patient.Contact.Add(contact);

      patient.Address.Add(
        new Address
          {
            Line = new[] { "3333 Dream Avenue, Suite 234" },
            City = "Dreamville",
            State = "DV",
            PostalCode = "12345",
            Country = "USA"
          });

      patient.Deceased = new FhirBoolean(false);

      return patient;
    }
  }
}