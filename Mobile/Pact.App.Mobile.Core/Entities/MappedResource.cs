namespace Pact.Fhir.Mobile.Entities
{
  using Hl7.Fhir.Serialization;

  using SQLite;

  using Resource = Hl7.Fhir.Model.Resource;

  public class MappedResource
  {
    [PrimaryKey, NotNull]
    public string Id { get; set; }

    [NotNull]
    public string Payload { get; set; }

    [NotNull]
    public string TypeName { get; set; }

    [NotNull]
    public string VersionId { get; set; }

    public static MappedResource FromResource(Resource resource)
    {
      return new MappedResource { Id = resource.Id, VersionId = resource.VersionId, TypeName = resource.TypeName, Payload = resource.ToJson() };
    }

    public Resource ToResource()
    {
      return new FhirJsonParser().Parse<Resource>(this.Payload);
    }
  }
}