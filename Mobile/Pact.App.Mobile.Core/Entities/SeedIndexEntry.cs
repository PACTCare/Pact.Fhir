namespace Pact.Fhir.Mobile.Entities
{
  using SQLite;

  public class SeedIndexEntry
  {
    [NotNull]
    public int Index { get; set; }

    [PrimaryKey]
    public string SeedHash { get; set; }
  }
}