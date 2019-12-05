namespace Pact.Fhir.Mobile.Entities
{
  using SQLite;

  public class ReferenceEntry
  {
    [PrimaryKey, NotNull]
    public string Reference { get; set; }

    [NotNull]
    public string Seed { get; set; }
  }
}